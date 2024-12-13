using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;

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

            using (ClientProxy proxy = new ClientProxy(binding, address))
            {
                proxy.TestConnection();
                proxy.CalculateEnergyConsumption("id");
                Console.ReadLine();
                
                proxy.UpdateEnergyConsumed("id", 0);
                Console.ReadLine();
                proxy.UpdateId("id1", "id2");
                Console.ReadLine();
                proxy.AddSmartMeter(new SmartMeter("","", 0,""));
                Console.ReadLine();
                proxy.DeleteSmartMeterById("id");
                Console.ReadLine();
                proxy.DeleteDatabase();
                Console.ReadLine();
                proxy.BackupDatabase();
                Console.ReadLine();

            }

            Console.ReadLine();
        }
    }
}
