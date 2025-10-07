using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.AppSettings
{
    public static class SettingFactory
    {

        public static string GetClientId { get; set; }
        public static string GetClientSecret { get; set; }
        public static string GetAuthority { get; set; }
        public static string GetCrmBaseUrl { get; set; }
        public static string GetCRMConnectionString { get; set; }
        public static string GetRunHour { get; set; }
        public static string GetRunMinute { get; set; }
        public static string GetTokenUrl { get; set; }
        public static string GetMaxRetries { get; set; }
        public static string GetUserName { get; set; }
        public static string GetPassword { get; set; }
        public static string GetTimeoutInSeconds { get; set; }
        public static string GetVersion { get; set; }
        public static string GetCallerObjectId { get; set; }
        public static string GetTotalCount { get; set; }

    }
}
