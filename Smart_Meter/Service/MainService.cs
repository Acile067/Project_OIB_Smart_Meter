using Common;
using Manager;
using Manager.AES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class MainService : IService
    {

        public void TestConnection()
        {
            try
            {
                Console.WriteLine("Success connected to Service");


                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nZahtev je prosledjen balanseru opterecenja.");

                proksi.TestConnectionLoadBalancer();
                kanal.Close();

                
            }
            catch { Console.WriteLine("Greska!!"); }
        }

        
        public double CalculateEnergyConsumption(byte[] encryptedId)
        {
            string secretKey = SecretKey.LoadKey(GetUserName() + ".txt");
            byte[] decriptedId= AES_Symm_Algorithm.DecryptData(secretKey, encryptedId);
            string meterId=DataConverter.BytesToString(decriptedId);
            
            //TO DO
            return 0;
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "ModifyEnergy")]
        public bool UpdateEnergyConsumed(byte[] meterId, byte[] newEnergyConsumed)
        {
            string id=DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), meterId));
            double energyConsumed = DataConverter.BytesToDouble(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), newEnergyConsumed));

            //TO DO
            return true;
            
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "ModifyId")]
        public bool UpdateId(byte[] meterId, byte[] newId)
        {
            string id1 = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), meterId));
            string id2 = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), newId));

            //TO DO
            return true;
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "AddEntity")]
        public bool AddSmartMeter(byte[] id, byte[] name, byte[] energy)
        {
            string newId = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), id));
            string newName = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), name));
            double newEnergy = DataConverter.BytesToDouble(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), energy));

            SmartMeter meter=new SmartMeter(newId, newName, newEnergy, "");
            
            //TO DO
            return true;
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "DeleteEntity")]
        public bool DeleteSmartMeterById(byte[] meterId)
        {
            string id = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), meterId));

            //TO DO
            return true;
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "DeleteDatabase")]
        public void DeleteDatabase()
        {
            //TO DO
            return;
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "ArchiveDatabase")]
        public void BackupDatabase()
        {
            //TO DO
            return;
        }


        public static string GetUserName()
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;
            return Formatter.ParseName(windowsIdentity.Name);
        }

        
    }
}
