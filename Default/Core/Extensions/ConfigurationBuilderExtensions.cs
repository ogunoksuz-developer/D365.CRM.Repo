using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Linq;

namespace LCW.Core.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static Serilog.ILogger ConfigureLogger(this IConfigurationBuilder builder)
        {
            var builtConfig = builder.Build();

            var elasticsearchSinkOptions = new ElasticsearchSinkOptions(
                           builtConfig[Keys.SerilogVariables_Nodes].Split(";").Select(uri => new Uri(uri)).ToList())
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                IndexFormat = builtConfig[Keys.SerilogVariables_IndexFormat],
                ModifyConnectionSettings = connectionConfiguration => connectionConfiguration.BasicAuthentication(
                              builtConfig[Keys.SerilogVariables_UserId], builtConfig[Keys.SerilogVariables_Password]),
                BufferBaseFilename = builtConfig[Keys.SerilogVariables_BufferBaseFilename],
                BufferFileSizeLimitBytes = long.Parse(builtConfig[Keys.SerilogVariables_BufferFileSizeLimitBytes]),
                BufferLogShippingInterval = new TimeSpan(long.Parse(builtConfig[Keys.SerilogVariables_BufferLogShippingInterval])),
                BufferRetainedInvalidPayloadsLimitBytes = long.Parse(builtConfig[Keys.SerilogVariables_BufferRetainedInvalidPayloadsLimitBytes]),
                BufferFileCountLimit = int.Parse(builtConfig[Keys.SerilogVariables_BufferFileCountLimit])
            };

            return new LoggerConfiguration()
                .ReadFrom.Configuration(builtConfig, new Serilog.Settings.Configuration.ConfigurationReaderOptions { SectionName = "Serilog" })
                .WriteTo.Elasticsearch(elasticsearchSinkOptions)
                .CreateLogger();
        }

        public static class Keys
        {
            public const string SerilogVariables_Nodes = "SerilogVariables:Nodes";
            public const string SerilogVariables_IndexFormat = "SerilogVariables:IndexFormat";
            public const string SerilogVariables_UserId = "SerilogVariables:UserId";
            public const string SerilogVariables_Password = "SerilogVariables:Password";
            public const string SerilogVariables_BufferBaseFilename = "SerilogVariables:BufferBaseFilename";
            public const string SerilogVariables_BufferFileSizeLimitBytes = "SerilogVariables:BufferFileSizeLimitBytes";
            public const string SerilogVariables_BufferLogShippingInterval = "SerilogVariables:BufferLogShippingInterval";
            public const string SerilogVariables_BufferRetainedInvalidPayloadsLimitBytes = "SerilogVariables:BufferRetainedInvalidPayloadsLimitBytes";
            public const string SerilogVariables_BufferFileCountLimit = "SerilogVariables:BufferFileCountLimit";
        }
    }
}
