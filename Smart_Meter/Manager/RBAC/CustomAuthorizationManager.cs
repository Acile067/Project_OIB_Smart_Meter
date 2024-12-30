using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Manager.RBAC
{
    public class CustomAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {

            CustomPrincipal principal = operationContext.ServiceSecurityContext.
                 AuthorizationContext.Properties["Principal"] as CustomPrincipal;
            string prefix = "http://tempuri.org/IService/";
            string operationName = OperationContext.Current.IncomingMessageHeaders.Action;
            // Find the start index after the prefix
            int startIndex = prefix.Length;
            string operation = operationName.Substring(startIndex);
            string userName = Formatter.ParseName(principal.Identity.Name);

            if(principal.IsInRole(operation))
            {
                return true;
            }
            else
            {
                try
                {
                    Audit.Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, operation + "method needs" + operation + "permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return false;
                
            }



        }
    }
}
