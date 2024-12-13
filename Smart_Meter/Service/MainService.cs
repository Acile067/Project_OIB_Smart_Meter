using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MainService : IService
    {
        public void TestConnection()
        {
            try
            {
                Console.WriteLine("Success connected to Service");


                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nZahtev je prosledjen balanseru opterecenja.");

                proksi.TestConnectionMainServiceLoadBalancer();
                kanal.Close();
            }
            catch { Console.WriteLine("Greska!!"); }
        }
        
        public double CalculateEnergyConsumption(string meterId)
        {
            //TO DO
            return 0;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "ModifyEnergy")]
        public bool UpdateEnergyConsumed(string meterId, double newEnergyConsumed)
        {
            return true;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "ModifyId")]
        public bool UpdateId(string meterId, string newId)
        {
            return true;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "AddEntity")]
        public bool AddSmartMeter(SmartMeter meter)
        {
            return true;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "DeleteEntity")]
        public bool DeleteSmartMeterById(string meterId)
        {
            return true;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "DeleteDatabase")]
        public void DeleteDatabase()
        {
            return;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "ArchiveDatabase")]
        public void BackupDatabase()
        {
            return;
        }

  

        

        
    }
}
