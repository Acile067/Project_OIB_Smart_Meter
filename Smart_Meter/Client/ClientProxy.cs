using Common;
using Manager.AES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientProxy : ChannelFactory<IService>, IService, IDisposable
    {
        IService factory;
        

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
            
        }

        public void TestConnection()
        {
            try
            {
                factory.TestConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
            }
        }

        public double CalculateEnergyConsumption(byte[] encryptedId)
        {
            double energyConsumption = 0;
            try
            {
                energyConsumption = factory.CalculateEnergyConsumption(encryptedId);
                Console.WriteLine("CalculateEnergyConsuption allowed!");
            }catch(Exception e)
            {
                Console.WriteLine("Error while trying to CalculateEnergyConsumption : {0}", e.Message);
            }
            return energyConsumption; //vrati rezultat
        }

        public bool UpdateEnergyConsumed(byte[] meterId, byte[] newEnergyConsumed)
        {
            bool updated = false;
            try
            {
                updated = factory.UpdateEnergyConsumed(meterId, newEnergyConsumed);  
                Console.WriteLine("UpdateEnergyConsumed allowed!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to UpdateEnergyConsumed : {0}", e.Message);
            }
            return updated;
        }


        public bool UpdateId(byte[] meterId, byte[] newId)
        {
            bool updated = false;
            try
            {
                updated = factory.UpdateId(meterId, newId);
                Console.WriteLine("UpdateId allowed!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to UpdateId : {0}", e.Message);
            }
            return updated; 
        }

        public bool AddSmartMeter(byte[] id, byte[]name, byte[]energy)
        {
            bool added = false;
            try
            {
                added = factory.AddSmartMeter(id, name, energy);
                Console.WriteLine("AddSmartMeter allowed!");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error while trying to AddSmartMeter : {0}", e.Message);
            }
            return added;
        }
        public bool DeleteSmartMeterById(byte[] meterId)
        {
            bool deleted = false;
            try
            {
                deleted = factory.DeleteSmartMeterById(meterId);
                Console.WriteLine("DeleteSmartMeterById allowed!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to DeleteSmartMeterById : {0}", e.Message);
            }
            return deleted;
        }

        public void DeleteDatabase()
        {
            try
            {
                factory.DeleteDatabase();
                Console.WriteLine("DeleteDatabase allowed!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to DeleteDatabase : {0}", e.Message);
            }
        }

        public void BackupDatabase()
        {
            try
            {
                factory.BackupDatabase();
                Console.WriteLine("BackupDatabase allowed!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to BackupDatabase : {0}", e.Message);
            }
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
