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

            string userName = Manager.Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string fileName = userName + ".txt";
            DeleteFile(fileName);  //ako je program zatvoren sa CTRL+C ili X ne obrise se fajl

            string eSecretKey = SecretKey.GenerateKey();
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
                            string meterId=InputId();
                            byte[] encryptedId = AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.StringToBytes(meterId));
                            proxy.CalculateEnergyConsumption(encryptedId);
                            break;

                        case 2:
                            Console.Write("Enter smart meter ID: ");
                            byte[] idToUpdate = AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.StringToBytes(InputId()));
                            Console.Write("Enter new energy consumed value: ");
                            double newEnergy = 0;
                            if(double.TryParse(Console.ReadLine(), out newEnergy))
                            {
                                proxy.UpdateEnergyConsumed(idToUpdate, AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.DoubleToBytes(newEnergy)));
                            }
                            else
                            {
                                Console.WriteLine("Invalid input of new energy consumed value!");
                            }
                            
                            break;

                        case 3:
                            Console.Write("Enter current smart meter ID: ");
                            byte[] oldId = AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.StringToBytes(InputId()));
                            Console.Write("Enter new smart meter ID: ");
                            byte[] newId = AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.StringToBytes(InputId()));
                            proxy.UpdateId(oldId, newId);
                            break;

                        case 4:
                            Console.Write("Enter smart meter details (ID, Name, Energy):\n");
                            string newMeterId = InputId();
                            string name1 = Console.ReadLine();
                            double energy1 = 0;
                            if (!double.TryParse(Console.ReadLine(), out energy1))
                            {
                                Console.WriteLine("Invalid input of energy!");
                                break;
                            }
                            byte[]id=AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.StringToBytes(newMeterId));
                            byte[]name=AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.StringToBytes(name1));
                            byte[] energy = AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.DoubleToBytes(energy1));
                            proxy.AddSmartMeter(id, name, energy);
                            break;

                        case 5:
                            Console.Write("Enter smart meter ID to delete: ");
                            byte[] idToDelete = AES_Symm_Algorithm.EncryptData(eSecretKey, DataConverter.StringToBytes(InputId())); 
                            proxy.DeleteSmartMeterById(idToDelete);
                            break;

                        case 6:
                            proxy.DeleteDatabase();
                            break;

                        case 7:
                            proxy.BackupDatabase();
                            break;

                        default:
                            DeleteFile(fileName);
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

        static string InputId()
        {
            string meterId = null;
            do
            {
                meterId = Console.ReadLine();

            } while (!IsValidId(meterId));
            return meterId;
        }


        static void DeleteFile(string fileName)
        {
            if (File.Exists("../../../Keys/" + fileName))
            {
                File.Delete("../../../Keys/" + fileName);
            }
            
        }

        

    }
}
