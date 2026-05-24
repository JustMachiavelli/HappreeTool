using Microsoft.Extensions.Configuration;

namespace HappreeTool.Configurations
{
    public static class ConfigurationLoader
    {
        public static IConfiguration LoadModuleConfiguration(string basePath,
                                                             string moduleName,
                                                             IConfiguration? baseConfiguration = null)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath);

            if (baseConfiguration != null)
            {
                builder.AddConfiguration(baseConfiguration);
            }

            return builder
                .AddJsonFile($"appsettings/appsettings.{moduleName}.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings/appsettings.{moduleName}.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}