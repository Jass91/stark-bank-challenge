using Stark.Application;
using Stark.Application.Invoice;
using Stark.Model;
using Stark.Application.Extensions;

namespace Stark.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddStark(this IServiceCollection services)
        {
            return services
                .AddWorkers()
                .AddStarkDependencies();
        }


        private static IServiceCollection AddWorkers(this IServiceCollection services)
        {
            services.AddHostedService(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var invoiceService = sp.GetRequiredService<IInvoiceService>()!;
                var logger = sp.GetRequiredService<ILogger<InvoiceGenerator>>()!;
                var invoiceConfig = config.GetSection("Workers:Invoice").Get<InvoiceWorkerConfig>()!;

                return new InvoiceGenerator(invoiceConfig, invoiceService, logger);
            });

            return services;
        }
    }
}
