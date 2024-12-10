using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MainService : IService
    {
        public void TestConnection()
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
    }
}
