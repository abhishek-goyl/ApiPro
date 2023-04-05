using Microsoft.Win32;
using Nippon.CommonLib.Models;
using Nippon.EnvironmentVariable;
using api.logging;
using Sapience.DB;
using System;

namespace Nippon.CommonLib
{
    public class DBUtill
    {        
        public static string GetNipponConnectionString(string db, string registryKey, string baseAppKey)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                string con = string.Empty;
                GlobalVariables gv = new GlobalVariables();
                if (gv.HasValue("runtime"))
                {
                    con = gv.GetValue(registryKey);
                }
                else
                {
                    RegistryKey baseKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\InnovizeTech\Sapience", false);
                    con = baseKey.GetValue(registryKey, string.Empty).ToString();
                }
                string dbConnectionString = Crypto.DecryptText(con);
                NipponConnectionString ncs = new NipponConnectionString(db, dbConnectionString);
                return ncs.ConnectionString;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
            }
            finally
            {
                log.Exit();
            }
            return string.Empty;
        }
        public static string GetNipponConnectionString(string db, string dbConnectionString)
        {
            NipponConnectionString ncs = new NipponConnectionString(db, dbConnectionString);
            return ncs.ConnectionString;
        }

        public static string GetNipponSecureConnectionString(string db, string dbConnectionString)
        {
            dbConnectionString = Crypto.DecryptText(dbConnectionString);
            NipponConnectionString ncs = new NipponConnectionString(db, dbConnectionString);
            return ncs.ConnectionString;
        }
    }
}
