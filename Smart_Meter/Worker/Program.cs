﻿using Common;
using System;
using System.Net.Sockets;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using Manager;
using Manager.Certificates;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace Worker
{
    public class Program
    {
        static void Main(string[] args)
        {
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();

            // Dinamički port se dodeljuje pre kreiranja servisa
            int port = GetFreeTcpPort();
            string baseAddress = $"net.tcp://localhost:{port}/Worker";

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // Kreiraj ServiceHost sa ručno generisanom baznom adresom
            ServiceHost host = new ServiceHost(typeof(WorkerService), new Uri(baseAddress));

            // Dodaj endpoint
            host.AddServiceEndpoint(typeof(IWorker), binding, "");

            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            // Otvori servis
            WorkerProxy workerProxy = null;
            try
            {
                host.Open();
                var name = WindowsIdentity.GetCurrent().Name;
                Console.WriteLine("[INFO] User - Worker: " + name);
                Console.WriteLine("[INFO] Worker is running.");
                Console.WriteLine($"[INFO] Assigned port: {port}");

                workerProxy = CreateWorkerProxy();
                bool ret = workerProxy.RegisterWorker(port, srvCertCN); //56732, Worker1
                if(ret)
                {
                    Console.WriteLine("[INFO] Succesfuly registered to Load Balancer.");
                }

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] " + e.Message);
                Console.WriteLine("[StackTrace] " + e.StackTrace);
            }
            finally
            {
                host.Close();
                workerProxy.RemoveWorker(port);
                workerProxy.Close();
                workerProxy.Dispose();
            }
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

        private static WorkerProxy CreateWorkerProxy()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9991/LoadBalancerRegisterWorkerService";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            return new WorkerProxy(binding, address);
        }
    }
}
