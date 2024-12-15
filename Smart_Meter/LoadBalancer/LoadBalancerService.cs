using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class LoadBalancerService : ILoadBalancer
    {
        public static ConcurrentDictionary<int, string> WorkerPortDict = new ConcurrentDictionary<int, string>();

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
            Console.WriteLine("Success connected to Load Balancer");
        }
    }
}
