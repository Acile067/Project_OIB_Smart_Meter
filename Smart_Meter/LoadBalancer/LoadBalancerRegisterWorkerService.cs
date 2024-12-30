using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class LoadBalancerRegisterWorkerService : IRegisterToLoadBalancer
    {
        public static ConcurrentDictionary<int, string> WorkerPortDict = new ConcurrentDictionary<int, string>();

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
    }
}
