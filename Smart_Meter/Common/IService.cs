using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void TestConnection();
        [OperationContract]
        double CalculateEnergyConsumption(byte[] encryptedId);

        [OperationContract]
        bool UpdateEnergyConsumed(byte[] meterId, byte[] newEnergyConsumed);
        [OperationContract]
        bool UpdateId(byte[] meterId, byte[] newId);
        [OperationContract]
        bool AddSmartMeter(byte[] id, byte[] name, byte[] energy);
        [OperationContract]
        bool DeleteSmartMeterById(byte[] meterId);
        [OperationContract]
        void DeleteDatabase();
        [OperationContract]
        void BackupDatabase();

        

    }
}
