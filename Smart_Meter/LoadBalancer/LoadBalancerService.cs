using Common;
using Manager.Certificates;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace LoadBalancer
{
    public class LoadBalancerService : ILoadBalancer
    {
        public static ConcurrentDictionary<int, string> WorkerPortDict = new ConcurrentDictionary<int, string>();
        private static int lastWorker = -1;

        public bool RegisterWorker(int port, string workerName)
        {
            if (WorkerPortDict.ContainsKey(port))
            {
                Console.WriteLine($"[ERROR] Port {port} is already registered by Worker: {WorkerPortDict[port]}.");
                return false;
            }

            if (WorkerPortDict.TryAdd(port, workerName))
            {
                Console.WriteLine($"[INFO] Worker '{workerName}' successfully registered on port {port}.");
                return true;
            }

            Console.WriteLine($"[ERROR] Failed to register Worker '{workerName}' on port {port}.");
            return false;
        }

        public bool RemoveWorker(int port)
        {
            if (WorkerPortDict.TryRemove(port, out string workerName))
            {
                Console.WriteLine($"[INFO] Worker '{workerName}' successfully removed from port {port}.");
                return true;
            }

            Console.WriteLine($"[ERROR] Failed to remove Worker. Port {port} is not registered.");
            return false;
        }

        public void TestConnectionLoadBalancer()
        {
            Console.WriteLine("[INFO] Successfully connected to Load Balancer.");
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
                lastWorker = (lastWorker + 1) % WorkerPortDict.Count;
            }
            else
            {
                Console.WriteLine("[INFO] No Workers available to perform the action.");
                return null;
            }

            int i = 0;
            foreach (var workerPort in WorkerPortDict.Keys)
            {
                if (i == lastWorker)
                {
                    string srvCertCN = WorkerPortDict[workerPort];
                    Console.WriteLine($"[INFO] Request {actionKey} sent to Worker: {srvCertCN} on port {workerPort}.");

                    var binding = new NetTcpBinding
                    {
                        Security = { Transport = { ClientCredentialType = TcpClientCredentialType.Certificate } }
                    };

                    var srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
                    var address = new EndpointAddress(new Uri($"net.tcp://localhost:{workerPort}/Worker"), new X509CertificateEndpointIdentity(srvCert));

                    using (var proxy = new LoadBalancerProxy(binding, address))
                    {
                        switch (actionKey)
                        {
                            case "TestConnectionLoadBalancer":
                                proxy.TestCommunicationWorker();
                                break;
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
                                Console.WriteLine("[ERROR] Unsupported action key.");
                                break;
                        }
                    }
                }
                i++;
            }

            return null;
        }
    }
}
