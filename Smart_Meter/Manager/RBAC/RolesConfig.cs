using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.IO;
using System.Collections;
using System.Reflection;

namespace Manager.RBAC
{
    public class RolesConfig
    {
        static readonly string path = @"~\..\..\..\..\Manager\RBAC\RolesConfigFile.resx";
        public static bool GetPermissions(string rolename, out string[] permissions)
        {
            permissions = new string[10];
            string permissionString = string.Empty;

            permissionString = (string)RolesConfigFile.ResourceManager.GetObject(rolename);
            if (permissionString != null)
            {
                permissions = permissionString.Split(',');
                return true;
            }
            return false;

        }
    }
}
