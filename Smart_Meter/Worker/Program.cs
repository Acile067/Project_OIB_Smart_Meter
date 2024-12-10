using Common;
using System;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;

namespace Worker
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:0/Worker"; // Dinamički port

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(WorkerService));
            host.AddServiceEndpoint(typeof(IWorker), binding, address);

            // Otvori servis pre nego što tražiš adresu
            host.Open();

            Console.WriteLine("User - Worker: " + WindowsIdentity.GetCurrent().Name);
            Console.WriteLine("Worker is running.");

            Console.ReadLine();
            host.Close();
        }
    }
}
