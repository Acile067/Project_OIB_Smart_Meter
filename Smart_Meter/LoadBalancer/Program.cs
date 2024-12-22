using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9998/LoadBalancer";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(LoadBalancerService));
            host.AddServiceEndpoint(typeof(ILoadBalancer), binding, address);

            host.Open();

            Console.WriteLine("[INFO] User - Load Blancer :" + WindowsIdentity.GetCurrent().Name);

            Console.WriteLine("[INFO] Load Balancer is running.");

            Console.ReadLine();
            host.Close();
        }
    }
}
