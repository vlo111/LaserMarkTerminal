using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;

namespace EzdDataL
{
    public class Load
    {
        public Load()
        {

        }

        public bool Go()
        {
            string configvalue1 = ConfigurationManager.AppSettings["Application"];

            var appPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            if (configvalue1 == null || configvalue1 != "15" || !File.Exists($@"{appPath}/app.config"))
            {
                return false;
            }

            int period = 2;
            string keyName = "System32";

            RegistryKey rootKey = Registry.CurrentUser;
            RegistryKey regKey = rootKey.OpenSubKey(keyName);

            if (regKey == null)
            {
                regKey = rootKey.CreateSubKey(keyName);
                long expiry = DateTime.Now.AddMinutes(period).Ticks;
                regKey.SetValue("driverW302", expiry, RegistryValueKind.QWord);
                regKey.Close();
            }
            else
            {
                long expiry = (long)regKey.GetValue("driverW302");
                regKey.Close();
                long today = DateTime.Now.Ticks;
                if (today > expiry)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
