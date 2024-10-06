using Microsoft.Extensions.Configuration;
using Stark.Model.Config;
using Stark.Application.Invoice;
using Microsoft.Extensions.DependencyInjection;

namespace Stark.Application.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // singleton pq é injetado no worker
            services.AddSingleton<IInvoiceService, InvoiceService>();

            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var starkConfig = config.GetSection("StarkAPI").Get<StarkApiConfig>()!;

                using(var fs = File.OpenRead(starkConfig.PrivateKeyFilePath))
                using(var sr = new StreamReader(fs))
                {
                    var privateKeyContent = sr.ReadToEnd();

                    StarkBank.Project project = new StarkBank.Project(
                        id: starkConfig.ProjectId,
                        privateKey: privateKeyContent,
                        environment: starkConfig.Environment
                    );

                    return project;
                }
            });

            return services;
        }
    }
}
