using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stark.API;
using Stark.API.Model.Config;
using Stark.Application.Invoice;

namespace Stark.Application.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddStarkDependencies(this IServiceCollection services)
        {
            return services
                .AddServices()
                .AddStarkAPIForProject(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var starkConfig = config.GetSection("StarkAPI").Get<StarkApiConfig>()!;

                    return starkConfig;
                });
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IInvoiceService, InvoiceService>();

            return services;
        }
    }
}
