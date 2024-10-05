using Microsoft.Extensions.DependencyInjection;
using Stark.API.Model.Config;
using Stark.API.Model.Enums;

namespace Stark.API
{
    public static class Extensions
    {
        public static IServiceCollection AddStarkAPIForProject(this IServiceCollection services, Func<IServiceProvider, StarkApiConfig> getConfig)
        {
            services.AddHttpClient("stark-api", (sp, client) =>
            {
                var starkConfig = getConfig(sp);
                client.BaseAddress = new Uri(starkConfig.Environment == StarkEnvironment.Sandbox ? "https://sandbox.api.starkbank.com" : "https://api.starkbank.com");
                client.DefaultRequestHeaders.Add("Access-ID", starkConfig.AccessId);
            });

            services.AddSingleton<IStarkBankApi>(sp =>
            {
                var starkConfig = getConfig(sp);
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return new StarkBankApi(starkConfig, factory.CreateClient("stark-api"));
            });

            return services;
        }

        private static string ReadPrivateKey(string pkFilePath) 
        {
            using(var s = File.OpenRead(pkFilePath))
            using(var reader = new StreamReader(s))
                return reader.ReadToEnd();
        }

    }
}
