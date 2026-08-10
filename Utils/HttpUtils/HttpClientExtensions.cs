using Microsoft.Extensions.DependencyInjection;

namespace HappreeTool.Utils.HttpUtils
{
    public static class HttpClientExtensions
    {
        public static readonly string MyApiClient = "MyApi";

        public static IServiceCollection AddMyApiHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<HttpWrapper>(MyApiClient)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    // 忽略 SSL 证书错误
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                });

            services.AddScoped<HttpWrapper>();
            return services;
        }
    }
}
