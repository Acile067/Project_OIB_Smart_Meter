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
                Console.WriteLine("[INFO] Energy calculated successfully!");
            }catch(Exception e)
            {
                Console.WriteLine("Error while trying to calculate energy : {0}", e.Message + "Not authorized to execute this command.");
            }
            return energyConsumption; //vrati rezultat
        }

        public bool UpdateEnergyConsumed(byte[] meterId, byte[] newEnergyConsumed)
        {
            bool updated = false;
            try
            {
                updated = factory.UpdateEnergyConsumed(meterId, newEnergyConsumed);
                Console.WriteLine("[INFO] Energy updated successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to update energy : {0}", e.Message + "Not authorized to execute this command.");
            }
            return updated;
        }


        public bool UpdateId(byte[] meterId, byte[] newId)
        {
            bool updated = false;
            try
            {
                updated = factory.UpdateId(meterId, newId);
                Console.WriteLine("[INFO] Smart Meter ID updated successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to UpdateId : {0}", e.Message + "Not authorized to execute this command.");
            }
            return updated; 
        }

        public bool AddSmartMeter(byte[] id, byte[]name, byte[]energy)
        {
            bool added = false;
            try
            {
                added = factory.AddSmartMeter(id, name, energy);
                Console.WriteLine("[INFO] SmartMeter added successfully!");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error while trying to add SmartMeter : {0}", e.Message + "Not authorized to execute this command.");
            }
            return added;
        }
        public bool DeleteSmartMeterById(byte[] meterId)
        {
            bool deleted = false;
            try
            {
                deleted = factory.DeleteSmartMeterById(meterId);
                Console.WriteLine("[INFO] SmartMeter deleted successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to delete smart meter : {0}", e.Message + "Not authorized to execute this command.");
            }
            return deleted;
        }

        public void DeleteDatabase()
        {
            try
            {
                factory.DeleteDatabase();
                Console.WriteLine("[INFO] Database deleted successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to delete database : {0}", e.Message + "Not authorized to execute this command.");
            }
        }

        public void BackupDatabase()
        {
            try
            {
                factory.BackupDatabase();
                Console.WriteLine("[INFO] Database backed up successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to backup database : {0}", e.Message + "Not authorized to execute this command.");
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
