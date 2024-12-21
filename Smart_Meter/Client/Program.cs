using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;
using Manager.AES;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/Service";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("User - Client : " + WindowsIdentity.GetCurrent().Name);

            string eSecretKey = SecretKey.GenerateKey();
            string userName = Manager.Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
         
            string fileName = userName + ".txt";
            SecretKey.StoreKey(eSecretKey, fileName);

            using (ClientProxy proxy = new ClientProxy(binding, address))
            {
                int choice;
                do
                {
                    choice = PrintMenu();

                    switch (choice)
                    {
                        case 1:
                            Console.Write("Enter smart meter ID: ");
                            string meterId;
                            do
                            {
                                meterId = Console.ReadLine();

                            } while (!IsValidId(meterId));
                            byte[] test = DataConverter.StringToBytes(meterId);
                            Console.WriteLine(test);
                            //byte[] encryptedId = AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.StringToBytes(meterId));

                            //proxy.CalculateEnergyConsumption(encryptedId);
                            break;

                        case 2:
                            Console.Write("Enter smart meter ID: ");
                            string idToUpdate = Console.ReadLine();
                            Console.Write("Enter new energy consumed value: ");
                            double newEnergy = double.Parse(Console.ReadLine());
                            proxy.UpdateEnergyConsumed(idToUpdate, newEnergy);
                            break;

                        case 3:
                            Console.Write("Enter current smart meter ID: ");
                            string oldId = Console.ReadLine();
                            Console.Write("Enter new smart meter ID: ");
                            string newId = Console.ReadLine();
                            proxy.UpdateId(oldId, newId);
                            break;

                        case 4:
                            Console.Write("Enter smart meter details (ID, Name, Energy, Location):\n");
                            string newMeterId = Console.ReadLine();
                            string name = Console.ReadLine();
                            double energy = double.Parse(Console.ReadLine());
                            string zone = Console.ReadLine();
                            proxy.AddSmartMeter(new SmartMeter(newMeterId, name, energy, zone));
                            break;

                        case 5:
                            Console.Write("Enter smart meter ID to delete: ");
                            string idToDelete = Console.ReadLine();
                            proxy.DeleteSmartMeterById(idToDelete);
                            break;

                        case 6:
                            proxy.DeleteDatabase();
                            Console.WriteLine("Database deleted successfully.");
                            break;

                        case 7:
                            proxy.BackupDatabase();
                            Console.WriteLine("Database backed up successfully.");
                            break;

                        default:
                            
                            if (File.Exists("../../../KEYS/"+fileName))
                            {
                                File.Delete("../../../KEYS/"+fileName);
                            }
                            Console.WriteLine("Exiting program...");
                            break;

                    }
                } while (choice != 8);
            }
            
          
        }
        static int PrintMenu()
        {
            Console.WriteLine("Please select an option:");
            Console.WriteLine("");
            Console.WriteLine("[ALL] 1. Calculate electricity consumption");
            Console.WriteLine("[OPERATOR] 2. Modify electricity consumption value of the smart meter");
            Console.WriteLine("[OPERATOR] 3. Modify the ID of the smart meter");
            Console.WriteLine("[ADMINISTRATOR] 4. Add a new smart meter");
            Console.WriteLine("[ADMINISTRATOR] 5. Delete a smart meter");
            Console.WriteLine("[SUPER-ADMINISTRATOR] 6. Delete the database");
            Console.WriteLine("[SUPER-ADMINISTRATOR]7. Archive the database");
            Console.WriteLine("");
            Console.WriteLine("8. Exit");

            int choice;
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out choice) && choice >= 1 && choice <= 8)
                {
                    return choice;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 8:");
                }
            }
            
        }


        static bool IsValidId(string id)
        {
            // Check if the string has exactly 8 characters
            if ((id.TrimStart('0').Length != 8) || (!int.TryParse(id, out int _)))
            {
                Console.WriteLine("Invalid input. Please try again!");
                return false;
            }

            return true;
        }

    }
}
