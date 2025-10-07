using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using PowerApps.Samples.Batch;
using Newtonsoft.Json;
using LCW.Core.LCW.Entities.Concrete;
using LCW.Core.AppSettings;

namespace LCW.Core.DataAccess.CDSWebApi
{
    /// <summary>
    /// Manages authentication and initializing samples using WebAPIService
    /// </summary>
    public static class App
    {
    
        /// <summary>
        /// Returns a Config to pass to the Service constructor.
        /// </summary>
        /// <returns></returns>
        public static Config InitializeApp()
        {
            //Used to configure the service
            Config config = new()
            {
                Url = SettingFactory.GetCrmBaseUrl,
                TokenUrl =SettingFactory.GetTokenUrl,
                ClientId = SettingFactory.GetClientId,
                ClientSecret = SettingFactory.GetClientSecret,
                MaxRetries = byte.Parse(SettingFactory.GetMaxRetries), //Default: 2
                UserName = SettingFactory.GetUserName,
                Password = SettingFactory.GetPassword,
                TimeoutInSeconds = ushort.Parse(SettingFactory.GetTimeoutInSeconds), //Default: 120
                Version = SettingFactory.GetVersion, //Default 9.2
                CallerObjectId = new Guid(SettingFactory.GetCallerObjectId), //Default empty Guid
                DisableCookies = false,
                TotalCount = !string.IsNullOrEmpty(SettingFactory.GetTotalCount) ? int.Parse(SettingFactory.GetTotalCount):50000
            };
            return config;
        }

    }
}

