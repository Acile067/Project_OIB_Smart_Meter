﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IWorker
    {
        [OperationContract]
        void TestCommunicationWorker();
        [OperationContract]
        double CalculateEnergyConsumption(string meterId);

        [OperationContract]
        bool UpdateEnergyConsumed(string meterId, double newEnergyConsumed);
        [OperationContract]
        bool UpdateId(string meterId, string newId);
        [OperationContract]
        bool AddSmartMeter(SmartMeter meter);
        [OperationContract]
        bool DeleteSmartMeterById(string meterId);
        [OperationContract]
        void DeleteDatabase();
        [OperationContract]
        void BackupDatabase();
    }
}
