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
            throw new NotImplementedException();
        }

        public bool UpdateId(string meterId, string newId)
        {
            throw new NotImplementedException();
        }
    }
}
