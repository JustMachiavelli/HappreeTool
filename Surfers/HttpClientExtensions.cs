using Microsoft.Extensions.DependencyInjection;

namespace HappreeTool.Surfers
{
    public static class HttpClientExtensions
    {
        public static readonly string MyApiClient = "MyApi";

        public static IServiceCollection AddMyApiHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<HttpClientWrapper>(MyApiClient)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    // 忽略 SSL 证书错误
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                });

            services.AddScoped<HttpClientWrapper>();
            return services;
        }
    }
}
