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

            NetTcpBinding binding2 = new NetTcpBinding();
            string address2 = "net.tcp://localhost:9991/LoadBalancerRegisterWorkerService";

            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding2.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host2 = new ServiceHost(typeof(LoadBalancerRegisterWorkerService));
            host2.AddServiceEndpoint(typeof(IRegisterToLoadBalancer), binding2, address2);

            host2.Open();

            Console.WriteLine("[INFO] User - Load Blancer :" + WindowsIdentity.GetCurrent().Name);

            Console.WriteLine("[INFO] Load Balancer is running.");

            Console.ReadLine();
            host.Close();
            host2.Close();
        }
    }
}
