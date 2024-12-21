using Common;
using Manager.Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using Manager;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LoadBalancer
{
    public class LoadBalancerProxy : ChannelFactory<IWorker>, IWorker, IDisposable
    {
        IWorker factory;

        public LoadBalancerProxy(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }

        public bool AddSmartMeter(SmartMeter meter)
        {
            bool ret = false;
            try
            {
                ret = factory.AddSmartMeter(meter);
            }
            catch (Exception e)
            {
                Console.WriteLine("[AddSmartMeter] ERROR = {0}", e.Message);
            }

            return ret;
            
        }

        public void BackupDatabase()
        {
            try
            {
                factory.BackupDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine("[BackupDatabase] ERROR = {0}", e.Message);
            }
        }

        public double CalculateEnergyConsumption(string meterId)
        {
            double ret = -1;
            try
            {
                ret = factory.CalculateEnergyConsumption(meterId);
            }
            catch (Exception e)
            {
                Console.WriteLine("[CalculateEnergyConsumption] ERROR = {0}", e.Message);
            }

            return ret;
        }

        public void DeleteDatabase()
        {
            try
            {
                factory.BackupDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine("[DeleteDatabase] ERROR = {0}", e.Message);
            }
        }

        public bool DeleteSmartMeterById(string meterId)
        {
            bool ret = false;
            try
            {
                ret = factory.DeleteSmartMeterById(meterId);
            }
            catch (Exception e)
            {
                Console.WriteLine("[DeleteSmartMeterById] ERROR = {0}", e.Message);
            }

            return ret;
        }

        public void TestCommunicationWorker()
        {
            try
            {
                factory.TestCommunicationWorker();
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestCommunicationWorker] ERROR = {0}", e.Message);
            }
        }

        public bool UpdateEnergyConsumed(string meterId, double newEnergyConsumed)
        {
            bool ret = false;
            try
            {
                ret = factory.UpdateEnergyConsumed(meterId, newEnergyConsumed);
            }
            catch (Exception e)
            {
                Console.WriteLine("[UpdateEnergyConsumed] ERROR = {0}", e.Message);
            }

            return ret;
        }

        public bool UpdateId(string meterId, string newId)
        {
            bool ret = false;
            try
            {
                ret = factory.UpdateId(meterId, newId);
            }
            catch (Exception e)
            {
                Console.WriteLine("[UpdateId] ERROR = {0}", e.Message);
            }

            return ret;
        }
    }
}
