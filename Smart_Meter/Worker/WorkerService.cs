using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class WorkerService : IWorker
    {
        public bool AddSmartMeter(SmartMeter meter)
        {
            throw new NotImplementedException();
        }

        public void BackupDatabase()
        {
            throw new NotImplementedException();
        }

        public double CalculateEnergyConsumption(string meterId)
        {
            throw new NotImplementedException();
        }

        public void DeleteDatabase()
        {
            throw new NotImplementedException();
        }

        public bool DeleteSmartMeterById(string meterId)
        {
            throw new NotImplementedException();
        }

        public void TestCommunicationWorker()
        {
            Console.WriteLine("[INFO] Successfuly connected to Worker");
        }

        public bool UpdateEnergyConsumed(string meterId, double newEnergyConsumed)
        {
            throw new NotImplementedException();
        }

        public bool UpdateId(string meterId, string newId)
        {
            throw new NotImplementedException();
        }
    }
}
