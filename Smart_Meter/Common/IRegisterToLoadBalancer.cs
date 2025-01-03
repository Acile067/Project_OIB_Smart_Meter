﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IRegisterToLoadBalancer
    {
        [OperationContract]
        bool RegisterWorker(int port, string workerName);
        [OperationContract]
        bool RemoveWorker(int port);
    }
}
