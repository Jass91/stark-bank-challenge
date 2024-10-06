using Stark.Application.Invoice;
using Stark.Model.Config;
using Stark.Workers;
using Stark.Application.Extensions;

namespace Stark
{
    public static class Extensions
    {
        public static IServiceCollection AddStarkDependencies(this IServiceCollection services)
        {
            services
                .AddServices()
                .AddHostedService(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var invoiceService = sp.GetRequiredService<IInvoiceService>();
                    var logger = sp.GetRequiredService<ILogger<InvoiceGenerator>>();

                    var workerConfig = config.GetSection("Workers:InvoiceGenerator").Get<InvoiceWorkerConfig>()!;

                    return new InvoiceGenerator(workerConfig, invoiceService, logger);
                });

            return services;
        }
    }
}
