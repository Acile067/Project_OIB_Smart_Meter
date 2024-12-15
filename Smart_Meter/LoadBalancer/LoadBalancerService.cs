using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class LoadBalancerService : ILoadBalancer
    {
        public bool RegisterWorker(int port)
        {
            return true;
        }

        public void TestConnectionLoadBalancer()
        {
            Console.WriteLine("Success connected to Load Balancer");
        }
    }
}
