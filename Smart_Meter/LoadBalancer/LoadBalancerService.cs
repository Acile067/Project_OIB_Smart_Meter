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
        public void RegisterWorker()
        {
            throw new NotImplementedException();
        }

        public void TestConnectionMainServiceLoadBalancer()
        {
            Console.WriteLine("Success connected to Load Balancer");
        }
    }
}
