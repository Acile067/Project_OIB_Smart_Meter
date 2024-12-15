using Common;
using System;
using System.Net.Sockets;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;

namespace Worker
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();

            // Dinamički port se dodeljuje pre kreiranja servisa
            int port = GetFreeTcpPort();
            string baseAddress = $"net.tcp://localhost:{port}/Worker";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            // Kreiraj ServiceHost sa ručno generisanom baznom adresom
            ServiceHost host = new ServiceHost(typeof(WorkerService), new Uri(baseAddress));

            // Dodaj endpoint
            host.AddServiceEndpoint(typeof(IWorker), binding, "");

            // Otvori servis
            host.Open();

            Console.WriteLine("User - Worker: " + WindowsIdentity.GetCurrent().Name);
            Console.WriteLine("Worker is running.");
            Console.WriteLine($"Assigned port: {port}");

            Console.ReadLine();
            host.Close();
        }

        // Funkcija za dobijanje slobodnog porta
        private static int GetFreeTcpPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
