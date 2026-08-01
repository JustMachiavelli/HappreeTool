using HappreeTool.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace HappreeTool.Logging
{
    public static class LoggingExtensions
    {
        public static IHostBuilder UseSerilogLogging(this IHostBuilder hostBuilder)
        {
            // 从配置获取日志配置
            IConfiguration moduleConfiguration = ConfigurationLoader.LoadModuleConfiguration(
                AppContext.BaseDirectory, "Infrastructure");

            // 配置应用日志的 Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(moduleConfiguration)
                .Enrich.FromLogContext()
                .Enrich.With<SimplifiedSourceContextEnricher>()
                .CreateLogger();

            hostBuilder.UseSerilog();

            return hostBuilder;
        }
    }

    internal class SimplifiedSourceContextEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? sourceContext))
            {
                var sourceContextString = sourceContext.ToString().Trim('"');
                if (sourceContextString.Contains('.'))
                {
                    var parts = sourceContextString.Split('.');
                    var simpleCategoryName = string.Join(".",
                        parts.Select((part, index) => index < parts.Length - 1 ? part[0].ToString() : part));
                    var simplifiedProperty = new LogEventProperty("SourceContext", new ScalarValue(simpleCategoryName));
                    logEvent.AddOrUpdateProperty(simplifiedProperty);
                }
            }
        }
    }

}