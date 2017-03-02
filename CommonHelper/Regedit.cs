using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using Microsoft.Win32;

namespace CommonHelper
{
    public class Regedit
    {
        public static object ReadParams(string regeditpath,string keyname)
        {
            Microsoft.Win32.RegistryKey _Registry =
                Microsoft.Win32.Registry.LocalMachine.CreateSubKey(regeditpath);             
            object result = _Registry.GetValue(keyname, false);
            _Registry.Close();
            return result;
        }

        public static bool WriteParams(string regeditpath, string keyname, object value, out string err)
        {
            err = "";
            try
            {
                Microsoft.Win32.RegistryKey _Registry =
                    Microsoft.Win32.Registry.LocalMachine.CreateSubKey(regeditpath);
                _Registry.SetValue(keyname, value);
                _Registry.Close();
                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }
    }
}
