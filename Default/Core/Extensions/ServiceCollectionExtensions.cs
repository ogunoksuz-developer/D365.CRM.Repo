using LCW.Core.AppSettings;
using LCW.Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCW.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyResolvers(this IServiceCollection services, ICoreModule[] modules)
        {
            foreach (var module in modules)
            {
                module.Load(services);
            }

            return ServiceTool.Create(services);
        }

        public static void AddLcwCoreServies(this IServiceCollection services, IConfiguration configuration)
        {
            SettingFactory.GetClientId = configuration.GetSection("ClientId").Value;
            SettingFactory.GetClientSecret = configuration.GetSection("ClientSecret").Value;
            SettingFactory.GetAuthority = configuration.GetSection("Authority").Value;
            SettingFactory.GetCrmBaseUrl = configuration.GetSection("Url").Value;
            SettingFactory.GetCRMConnectionString = configuration.GetSection("CRMConnectionString").Value;
            SettingFactory.GetTokenUrl = configuration.GetSection("TokenUrl").Value;
            SettingFactory.GetMaxRetries = configuration.GetSection("MaxRetries").Value;
            SettingFactory.GetUserName = configuration.GetSection("CRMUserName").Value;
            SettingFactory.GetPassword = configuration.GetSection("Password").Value;
            SettingFactory.GetTimeoutInSeconds = configuration.GetSection("TimeoutInSeconds").Value;
            SettingFactory.GetVersion = configuration.GetSection("Version").Value;
            SettingFactory.GetCallerObjectId = configuration.GetSection("CallerObjectId").Value;
            SettingFactory.GetTotalCount = configuration.GetSection("TotalCount").Value;
            SettingFactory.GetRunHour = configuration.GetSection("RunHour").Value;
            SettingFactory.GetRunMinute = configuration.GetSection("RunMinute").Value;
        }
    }
}
