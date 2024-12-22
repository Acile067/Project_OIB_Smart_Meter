using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class WorkerProxy : ChannelFactory<ILoadBalancer>, ILoadBalancer, IDisposable
    {
        ILoadBalancer factory;

        public WorkerProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public bool RegisterWorker(int port, string workerName)
        {
            bool ret = false;
            try
            {
                ret = factory.RegisterWorker(port, workerName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[RegisterWorker] ERROR = {0}", e.Message);
            }

            return ret;    
        }

        public void TestConnectionLoadBalancer()
        {
            try
            {
                factory.TestConnectionLoadBalancer();
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestConnectionLoadBalancer] ERROR = {0}", e.Message);
            }
        }
        public bool RemoveWorker(int port)
        {
            bool ret = false;
            try
            {
                ret = factory.RemoveWorker(port);
            }
            catch (Exception e)
            {
                Console.WriteLine("[RegisterWorker] ERROR = {0}", e.Message);
            }

            return ret;
        }
        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }

        public double CalculateEnergyConsumption(string meterId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEnergyConsumed(string meterId, double newEnergyConsumed)
        {
            throw new NotImplementedException();
        }

        public bool UpdateId(string meterId, string newId)
        {
            throw new NotImplementedException();
        }

        public bool AddSmartMeter(SmartMeter meter)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSmartMeterById(string meterId)
        {
            throw new NotImplementedException();
        }

        public void DeleteDatabase()
        {
            throw new NotImplementedException();
        }

        public void BackupDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
