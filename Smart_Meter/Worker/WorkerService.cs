using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Worker
{
    public class WorkerService : IWorker
    {
        private static readonly string DatabaseFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "DataBase");
        private static readonly string DatabaseFilePath = Path.Combine(DatabaseFolderPath, "smart_meters.json");
        private static readonly object fileLock = new object();
        private static Dictionary<string, SmartMeter> meters;

        // Staticki konstruktor
        static WorkerService()
        {
            try
            {
                Console.WriteLine("[INFO] Static constructor called.");
                EnsureDatabaseFolderExists();
                LoadDatabase();
                Console.WriteLine("[INFO] Initialization completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Initialization failed: " + ex.Message);
                Console.WriteLine("[StackTrace] " + ex.StackTrace);
                meters = new Dictionary<string, SmartMeter>(); // Podesi default vrednost
            }
        }

        private static void EnsureDatabaseFolderExists()
        {
            if (!Directory.Exists(DatabaseFolderPath))
            {
                Console.WriteLine("[INFO] Creating database folder.");
                Directory.CreateDirectory(DatabaseFolderPath);
            }
        }

        private static void LoadDatabase()
        {
            lock (fileLock)
            {
                if (File.Exists(DatabaseFilePath))
                {
                    Console.WriteLine("[INFO] Loading database from file.");
                    var json = File.ReadAllText(DatabaseFilePath);
                    meters = JsonSerializer.Deserialize<Dictionary<string, SmartMeter>>(json) ?? new Dictionary<string, SmartMeter>();

                    // Čišćenje podataka
                    foreach (var key in new List<string>(meters.Keys))
                    {
                        var meter = meters[key];
                        meter.MeterId = meter.MeterId.TrimEnd('\0');
                        meter.OwnerName = meter.OwnerName.TrimEnd('\0');

                        // Zamenjujemo ključ ako je obrisano '\0'
                        if (key != meter.MeterId)
                        {
                            meters.Remove(key);
                            meters[meter.MeterId] = meter;
                        }
                    }
                    Console.WriteLine("[INFO] Database loaded successfully.");
                }
                else
                {
                    Console.WriteLine("[DEBUG] Database file does not exist, initializing new database.");
                    meters = new Dictionary<string, SmartMeter>();
                }
            }
        }

        private static void SaveDatabase()
        {
            lock (fileLock)
            {
                try
                {
                    var json = JsonSerializer.Serialize(meters);
                    File.WriteAllText(DatabaseFilePath, json);
                    Console.WriteLine("[INFO] Database saved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Failed to save database: " + ex.Message);
                }
            }
        }

        public bool AddSmartMeter(SmartMeter meter)
        {
            lock (fileLock)
            {
                meter.MeterId = meter.MeterId.TrimEnd('\0');
                meter.OwnerName = meter.OwnerName.TrimEnd('\0');

                if (meters.ContainsKey(meter.MeterId))
                {
                    Console.WriteLine($"[ERROR] MeterId '{meter.MeterId}' already exists in the database.");
                    return false;
                }

                meters[meter.MeterId] = meter;
                SaveDatabase();
                Console.WriteLine($"[INFO] MeterId '{meter.MeterId}' successfully added to the database.");
                return true;
            }
        }

        public void BackupDatabase()
        {
            lock (fileLock)
            {
                try
                {
                    string archiveFilePath = Path.Combine(DatabaseFolderPath, $"archive_{DateTime.Now:yyyyMMddHHmmss}.json");
                    File.Copy(DatabaseFilePath, archiveFilePath, overwrite: true);
                    Console.WriteLine($"[INFO] Database backup created at '{archiveFilePath}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Failed to backup database: " + ex.Message);
                }
            }
        }

        public double CalculateEnergyConsumption(string meterId)
        {
            lock (fileLock)
            {
                string cleanMeterId = meterId.TrimEnd('\0');
                Console.WriteLine($"[DEBUG] Calculating energy consumption for MeterId: '{cleanMeterId}'.");

                return meters.TryGetValue(cleanMeterId, out var meter) ? meter.EnergyConsumed : 0.0;
            }
        }

        public void DeleteDatabase()
        {
            lock (fileLock)
            {
                if (File.Exists(DatabaseFilePath))
                {
                    Console.WriteLine("[INFO] Deleting database file.");
                    File.Delete(DatabaseFilePath);
                }
                meters.Clear();
                Console.WriteLine("[INFO] Database cleared.");
            }
        }

        public bool DeleteSmartMeterById(string meterId)
        {
            lock (fileLock)
            {
                string cleanMeterId = meterId.TrimEnd('\0');
                Console.WriteLine($"[DEBUG] Attempting to delete MeterId: '{cleanMeterId}'.");

                if (!meters.ContainsKey(cleanMeterId))
                {
                    Console.WriteLine($"[ERROR] MeterId '{cleanMeterId}' not found in database.");
                    return false;
                }

                meters.Remove(cleanMeterId);
                SaveDatabase();
                Console.WriteLine($"[INFO] MeterId '{cleanMeterId}' successfully deleted.");
                return true;
            }
        }

        public void TestCommunicationWorker()
        {
            Console.WriteLine("[INFO] Successfully connected to Worker.");
        }

        public bool UpdateEnergyConsumed(string meterId, double newEnergyConsumed)
        {
            lock (fileLock)
            {
                string cleanMeterId = meterId.TrimEnd('\0');
                Console.WriteLine($"[DEBUG] Updating energy consumed for MeterId: '{cleanMeterId}'.");

                if (!meters.ContainsKey(cleanMeterId))
                {
                    Console.WriteLine($"[ERROR] MeterId '{cleanMeterId}' not found in database.");
                    return false;
                }

                meters[cleanMeterId].EnergyConsumed = newEnergyConsumed;
                SaveDatabase();
                Console.WriteLine($"[INFO] Energy consumption for MeterId '{cleanMeterId}' successfully updated.");
                return true;
            }
        }

        public bool UpdateId(string meterId, string newId)
        {
            lock (fileLock)
            {
                string cleanMeterId = meterId.TrimEnd('\0');
                string cleanNewId = newId.TrimEnd('\0');
                Console.WriteLine($"[DEBUG] Updating MeterId '{cleanMeterId}' to '{cleanNewId}'.");

                if (!meters.ContainsKey(cleanMeterId) || meters.ContainsKey(cleanNewId))
                {
                    Console.WriteLine($"[ERROR] Update failed. Either MeterId '{cleanMeterId}' does not exist or newId '{cleanNewId}' already exists.");
                    return false;
                }

                var meter = meters[cleanMeterId];
                meters.Remove(cleanMeterId);
                meter.MeterId = cleanNewId;
                meters[cleanNewId] = meter;
                SaveDatabase();
                Console.WriteLine($"[INFO] MeterId '{cleanMeterId}' successfully updated to '{cleanNewId}'.");
                return true;
            }
        }
    }
}