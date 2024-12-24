using Common;
using Manager;
using Manager.AES;
using Manager.Audit;
using Manager.RBAC;
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

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            try
            {
                Audit.AuthorizationSuccess(userName,
                    OperationContext.Current.IncomingMessageHeaders.Action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            string secretKey = SecretKey.LoadKey(GetUserName() + ".txt");
            byte[] decriptedId= AES_Symm_Algorithm.DecryptData(secretKey, encryptedId);
            string meterId=DataConverter.BytesToString(decriptedId);
            double ret = -1;
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nZahtev je prosledjen balanseru opterecenja.");           

                ret = proksi.CalculateEnergyConsumption(meterId);

                //Console.WriteLine("Calculate energy consumption successfully executed.");

                

                kanal.Close();
            }
            catch { Console.WriteLine("Greska!!"); }

            return ret;
        }


        //[PrincipalPermission(SecurityAction.Demand, Role = "ModifyEnergy")]
        public bool UpdateEnergyConsumed(byte[] meterId, byte[] newEnergyConsumed)
        {

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("ModifyEnergy"))
            {
                //Console.WriteLine("UpdateEnergyConsumed successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "UpdateEnergyConsumed method needs ModifyEnergy permission.");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                throw new FaultException("User " + userName +
                    " try to call UpdateEnergyConsumed method. UpdateEnergyConsumed method needs ModifyEnergy permission.");
            }

            string id=DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), meterId));
            double energyConsumed = DataConverter.BytesToDouble(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), newEnergyConsumed));

            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nZahtev je prosledjen balanseru opterecenja.");

                proksi.UpdateEnergyConsumed(id, energyConsumed);              

                kanal.Close();
            }
            catch { Console.WriteLine("Greska!!"); }

            return true;
            
        }


        //[PrincipalPermission(SecurityAction.Demand, Role = "ModifyId")]
        public bool UpdateId(byte[] meterId, byte[] newId)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            /*Console.WriteLine($"User {userName} attempting to call UpdateId");
            Console.WriteLine("Authorization success: " + AuditEvents.AuthorizationSuccess);
            Console.WriteLine("Authorization failed: " + AuditEvents.AuthorizationFailure);
            Console.WriteLine("Auth success: " + AuditEvents.AuthenticationSuccess);
            string prefix = "http://tempuri.org/IService/";
            string operationName = OperationContext.Current.IncomingMessageHeaders.Action;
            // Find the start index after the prefix
            int startIndex = prefix.Length;
            string operation = operationName.Substring(startIndex);
            Console.WriteLine(operation);*/
            bool isInRole = Thread.CurrentPrincipal.IsInRole("ModifyId");
            Console.WriteLine($"User is in ModifyId role: {isInRole}");

            if (isInRole)
            {
                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                    Console.WriteLine("Authorization success logged: " + AuditEvents.AuthorizationSuccess);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error logging authorization success: {e.Message}");
                }
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "UpdateId method needs ModifyId permission.");
                    Console.WriteLine("Authorization failure logged");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Username: " + userName);
                    
                    Console.WriteLine("Authorization failure message: " + AuditEvents.AuthorizationFailure);
                    Console.WriteLine($"Error logging authorization failure: {e.Message}");
                }

                throw new FaultException("User " + userName +
                    " tried to call UpdateId method. UpdateId method needs ModifyId permission.");
            }

            string id1 = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), meterId));
            string id2 = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), newId));

            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nRequest has been forwarded to the load balancer.");

                proksi.UpdateId(id1, id2);

                kanal.Close();
            }
            catch
            {
                Console.WriteLine("Error occurred while calling load balancer.");
            }

            return true;
        }




        //[PrincipalPermission(SecurityAction.Demand, Role = "AddEntity")]
        public bool AddSmartMeter(byte[] id, byte[] name, byte[] energy)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);
            /*string prefix = "http://tempuri.org/IService/";
            string operationName = OperationContext.Current.IncomingMessageHeaders.Action;
            // Find the start index after the prefix
            int startIndex = prefix.Length;
            string operation = operationName.Substring(startIndex);
            Console.WriteLine(operation);*/

            if (Thread.CurrentPrincipal.IsInRole("AddEntity"))
            {
                Console.WriteLine("User is in AddEntity mode");
                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                Console.WriteLine("User is NOT in AddEntity mode");
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "AddSmartMeter method needs AddEntity permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call AddSmartMeter method. AddSmartMeter method needs AddEntity permission.");
            }


            string newId = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), id));
            string newName = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), name));
            double newEnergy = DataConverter.BytesToDouble(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), energy));

            SmartMeter meter=new SmartMeter(newId, newName, newEnergy, "");

            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nZahtev je prosledjen balanseru opterecenja.");

                proksi.AddSmartMeter(meter);

               

                kanal.Close();
            }
            catch { Console.WriteLine("Greska!!"); }
          
            return true;
        }


        //[PrincipalPermission(SecurityAction.Demand, Role = "DeleteEntity")]
        public bool DeleteSmartMeterById(byte[] meterId)
        {

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("DeleteEntity"))
            {
                //Console.WriteLine("DeleteSmartMeterById successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "DeleteSmartMeterById method needs DeleteEntity permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call DeleteSmartMeterById method. DeleteSmartMeterById method needs DeleteEntity permission.");
            }

            string id = DataConverter.BytesToString(AES_Symm_Algorithm.DecryptData(SecretKey.LoadKey(GetUserName() + ".txt"), meterId));


            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nZahtev je prosledjen balanseru opterecenja.");

                proksi.DeleteSmartMeterById(id);
                              
                kanal.Close();
            }
            catch { Console.WriteLine("Greska!!"); }
            return true;
        }


        //[PrincipalPermission(SecurityAction.Demand, Role = "DeleteDatabase")]
        public void DeleteDatabase()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("DeleteDatabase"))
            {
                //Console.WriteLine("DeleteDatabase successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "DeleteDatabase method needs DeleteDatabase permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call DeleteDatabase method. DeleteDatabase method needs DeleteDatabase permission.");
            }
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nZahtev je prosledjen balanseru opterecenja.");

                proksi.DeleteDatabase();                

                kanal.Close();
            }
            catch { Console.WriteLine("Greska!!"); }
            return;
        }


        //[PrincipalPermission(SecurityAction.Demand, Role = "ArchiveDatabase")]
        public void BackupDatabase()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("ArchiveDatabase"))
            {

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "BackupDatabase method needs ArchiveDatabase permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call BackupDatabase method. BackupDatabase method needs ArchiveDatabase permission.");
            }
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string adresa = "net.tcp://localhost:9998/LoadBalancer";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

                ChannelFactory<ILoadBalancer> kanal = new ChannelFactory<ILoadBalancer>(binding, new EndpointAddress(adresa));
                ILoadBalancer proksi = kanal.CreateChannel();

                Console.WriteLine("\nZahtev je prosledjen balanseru opterecenja.");

                proksi.BackupDatabase();               

                kanal.Close();
            }
            catch { Console.WriteLine("Greska!!"); }
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
