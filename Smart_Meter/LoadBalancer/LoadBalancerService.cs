using Common;
using Manager.Certificates;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class LoadBalancerService : ILoadBalancer
    {
        public static ConcurrentDictionary<int, string> WorkerPortDict = new ConcurrentDictionary<int, string>();
        private static int lastWorker = -1;

        public bool RegisterWorker(int port, string workerName)
        {
            // Proveri da li port već postoji u rečniku
            if (WorkerPortDict.ContainsKey(port))
            {
                Console.WriteLine($"[ERROR] Port {port} is already registered by Worker: {WorkerPortDict[port]}.");
                return false; // Neuspeh registracije
            }

            // Dodaj port i ime Workera u rečnik
            if (WorkerPortDict.TryAdd(port, workerName))
            {
                Console.WriteLine($"[INFO] Worker '{workerName}' successfully registered on port {port}.");
                return true; // Uspešna registracija
            }

            Console.WriteLine($"[ERROR] Failed to register Worker '{workerName}' on port {port}.");
            return false; // Dodavanje nije uspelo
        }

        public bool RemoveWorker(int port)
        {
            // Pokušaj uklanjanja porta iz rečnika
            if (WorkerPortDict.TryRemove(port, out string workerName))
            {
                Console.WriteLine($"[INFO] Worker '{workerName}' successfully removed from port {port}.");
                return true; // Uklanjanje uspešno
            }

            // Port nije pronađen u rečniku
            Console.WriteLine($"[ERROR] Failed to remove Worker. Port {port} is not registered.");
            return false; // Neuspešno uklanjanje
        }

        public void TestConnectionLoadBalancer()
        {
            Console.WriteLine("[INFO] Successfuly connected to Load Balancer");

            if(WorkerPortDict.Count > 0)
            {
                lastWorker = (lastWorker+1)% (WorkerPortDict.Count);
            }
            else
            {
                lastWorker = -1;
            }

            if (WorkerPortDict.Count > 0) 
            {
                int i = 0;
                foreach (int workerPort in WorkerPortDict.Keys)
                {
                    if(i == lastWorker)
                    {
                        string srvCertCN = WorkerPortDict[workerPort];
                        Console.WriteLine("[INFO] Request sent to: " + srvCertCN);

                        NetTcpBinding binding = new NetTcpBinding();
                        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                        X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
                        EndpointAddress address = new EndpointAddress(new Uri($"net.tcp://localhost:{workerPort}/Worker"),
                                                  new X509CertificateEndpointIdentity(srvCert));

                        using(LoadBalancerProxy proxy = new LoadBalancerProxy(binding, address))
                        {
                            proxy.TestCommunicationWorker();
                            Console.WriteLine("[INFO] TestCommunication() finished.");
                        }
                    }
                    i++;
                }
            }
            else
            {
                Console.WriteLine("[INFO] No Workes To Do Job");
            }
        }
    }
}
