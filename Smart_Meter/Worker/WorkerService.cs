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
                Directory.CreateDirectory(DatabaseFolderPath);
            }
        }

        private static void LoadDatabase()
        {
            lock (fileLock)
            {
                if (File.Exists(DatabaseFilePath))
                {
                    var json = File.ReadAllText(DatabaseFilePath);
                    meters = JsonSerializer.Deserialize<Dictionary<string, SmartMeter>>(json) ?? new Dictionary<string, SmartMeter>();
                }
                else
                {
                    meters = new Dictionary<string, SmartMeter>();
                }
            }
        }

        private static void SaveDatabase()
        {
            lock (fileLock)
            {
                var json = JsonSerializer.Serialize(meters);
                File.WriteAllText(DatabaseFilePath, json);
            }
        }

        public bool AddSmartMeter(SmartMeter meter)
        {
            lock (fileLock)
            {
                if (meters.ContainsKey(meter.MeterId)) return false;
                meters[meter.MeterId] = meter;
                SaveDatabase();
                return true;
            }
        }

        public void BackupDatabase()
        {
            lock (fileLock)
            {
                string archiveFilePath = Path.Combine(DatabaseFolderPath, $"archive_{DateTime.Now:yyyyMMddHHmmss}.json");
                File.Copy(DatabaseFilePath, archiveFilePath, overwrite: true);
            }
        }

        public double CalculateEnergyConsumption(string meterId)
        {
            lock (fileLock)
            {
                return meters.TryGetValue(meterId, out var meter) ? meter.EnergyConsumed : 0.0;
            }
        }

        public void DeleteDatabase()
        {
            lock (fileLock)
            {
                if (File.Exists(DatabaseFilePath))
                {
                    File.Delete(DatabaseFilePath);
                    meters.Clear();
                }
            }
        }

        public bool DeleteSmartMeterById(string meterId)
        {
            lock (fileLock)
            {
                if (!meters.Remove(meterId)) return false;
                SaveDatabase();
                return true;
            }
        }

        public void TestCommunicationWorker()
        {
            Console.WriteLine("[INFO] Successfuly connected to Worker");
        }

        public bool UpdateEnergyConsumed(string meterId, double newEnergyConsumed)
        {
            lock (fileLock)
            {
                if (!meters.ContainsKey(meterId)) return false;
                meters[meterId].EnergyConsumed = newEnergyConsumed;
                SaveDatabase();
                return true;
            }
        }

        public bool UpdateId(string meterId, string newId)
        {
            lock (fileLock)
            {
                if (!meters.ContainsKey(meterId) || meters.ContainsKey(newId)) return false;
                var meter = meters[meterId];
                meters.Remove(meterId);
                meter.MeterId = newId;
                meters[newId] = meter;
                SaveDatabase();
                return true;
            }
        }
    }
}
