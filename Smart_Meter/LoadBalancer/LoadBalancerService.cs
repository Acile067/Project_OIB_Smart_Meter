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

            ExecuteActionOnWorker("TestConnectionLoadBalancer", null);
        }

        public bool UpdateEnergyConsumed(string meterId, double newEnergyConsumed)
        {
            return (bool)ExecuteActionOnWorker("UpdateEnergyConsumed", new object[] { meterId, newEnergyConsumed });
        }

        public bool UpdateId(string meterId, string newId)
        {
            return (bool)ExecuteActionOnWorker("UpdateId", new object[] { meterId, newId });
        }

        public bool AddSmartMeter(SmartMeter meter)
        {
            return (bool)ExecuteActionOnWorker("AddSmartMeter", new object[] { meter });
        }

        public void BackupDatabase()
        {
            ExecuteActionOnWorker("BackupDatabase", null);
        }

        public double CalculateEnergyConsumption(string meterId)
        {
            return (double)ExecuteActionOnWorker("CalculateEnergyConsumption", new object[] { meterId });
        }

        public void DeleteDatabase()
        {
            ExecuteActionOnWorker("DeleteDatabase", null);
        }

        public bool DeleteSmartMeterById(string meterId)
        {
            return (bool)ExecuteActionOnWorker("DeleteSmartMeterById", new object[] { meterId });
        }

        private object ExecuteActionOnWorker(string actionKey, object[] args)
        {
            if (WorkerPortDict.Count > 0)
            {
                lastWorker = (lastWorker + 1) % (WorkerPortDict.Count);
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
                    if (i == lastWorker)
                    {
                        string srvCertCN = WorkerPortDict[workerPort];
                        Console.WriteLine($"[INFO] Request sent to: {srvCertCN}");

                        NetTcpBinding binding = new NetTcpBinding();
                        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                        X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
                        EndpointAddress address = new EndpointAddress(new Uri($"net.tcp://localhost:{workerPort}/Worker"),
                                                                      new X509CertificateEndpointIdentity(srvCert));

                        using (LoadBalancerProxy proxy = new LoadBalancerProxy(binding, address))
                        {
                            // Depending on the action, invoke the appropriate method
                            switch (actionKey)
                            {
                                case "TestConnectionLoadBalancer":
                                    proxy.TestCommunicationWorker();
;                                   break;
                                case "UpdateEnergyConsumed":
                                    return proxy.UpdateEnergyConsumed((string)args[0], (double)args[1]);
                                case "UpdateId":
                                    return proxy.UpdateId((string)args[0], (string)args[1]);
                                case "AddSmartMeter":
                                    return proxy.AddSmartMeter((SmartMeter)args[0]);
                                case "BackupDatabase":
                                    proxy.BackupDatabase();
                                    break;
                                case "CalculateEnergyConsumption":
                                    return proxy.CalculateEnergyConsumption((string)args[0]);
                                case "DeleteDatabase":
                                    proxy.DeleteDatabase();
                                    break;
                                case "DeleteSmartMeterById":
                                    return proxy.DeleteSmartMeterById((string)args[0]);
                                default:
                                    Console.WriteLine("[ERROR] Action not supported.");
                                    break;
                            }
                        }
                    }
                    i++;
                }
            }
            else
            {
                Console.WriteLine("[INFO] No Workers To Perform the Action");
            }       
            return null;
        }
    }
}
