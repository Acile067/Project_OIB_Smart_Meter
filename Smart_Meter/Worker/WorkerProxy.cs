﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class WorkerProxy : ChannelFactory<IRegisterToLoadBalancer>, IRegisterToLoadBalancer, IDisposable
    {
        IRegisterToLoadBalancer factory;

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
    }
}
