using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
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

            ServiceHost host = new ServiceHost(typeof(MainService));
            host.AddServiceEndpoint(typeof(IService), binding, address);

            host.Open();

            Console.WriteLine("User - MainService: " + WindowsIdentity.GetCurrent().Name);

            Console.WriteLine("MainService is running.");

            Console.ReadLine();
            host.Close();
        }
    }
}
