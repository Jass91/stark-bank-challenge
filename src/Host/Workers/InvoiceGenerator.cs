
using Stark.Model.Config;
using Stark.Application.Invoice;

namespace Stark.Workers
{
    public class InvoiceGenerator : BackgroundService
    {
        private int executionCount = 1;
        private readonly PeriodicTimer _timer;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceGenerator> _logger;
        private readonly InvoiceWorkerConfig _invoiceWorkerConfig;

        public InvoiceGenerator
        (
            InvoiceWorkerConfig config,
            IInvoiceService invoiceService,
            ILogger<InvoiceGenerator> logger
        )
        {
            _logger = logger;
            _invoiceWorkerConfig = config;
            _invoiceService = invoiceService;

            _timer = new PeriodicTimer(TimeSpan.FromHours(_invoiceWorkerConfig.IntervalHours));
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            _timer?.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Invoice worker running.");

            do
            {
                // roda a cada 3 horas durante 24h, entao, precisa executar 8 vezes (8 * 3 = 24)
                if (executionCount > 8)
                {
                    _logger.LogInformation("24 hours cycle completed.");
                    break;
                }

                CreateInvoices();

            } while (await _timer.WaitForNextTickAsync(stoppingToken));

            await StopAsync(stoppingToken);

            _logger.LogInformation("Invoice Generator is stopped");
        }


        private void CreateInvoices()
        {
            _logger.LogInformation("Creating Invoices for execution #{Execution}...", executionCount);

            var rnd = new Random(DateTime.UtcNow.Second);
            var idx = rnd.Next(0, 1);
            var max = ((int[])[8, 12])[idx];
            var invoices = new List<StarkBank.Invoice>();
            foreach (var i in Enumerable.Range(1, max))
                invoices.Add(MockInvoiceModel(rnd));

            try
            {
                var createdInvoices = _invoiceService.Create(invoices);
                executionCount++;
                // var count = Interlocked.Increment(ref executionCount);
            }
            catch (Exception ex)
            {
                executionCount--;

                _logger.LogError(ex, "Error trying to create invoices");
            }
        }

        private StarkBank.Invoice MockInvoiceModel(Random rnd)
        {
            var clilents = _invoiceWorkerConfig.Clients;
            var client = clilents[rnd.Next(0, clilents.Length - 1)];

            return new StarkBank.Invoice(
                name: client.Name,
                taxID: client.TaxID,
                amount: rnd.NextInt64(100000),
                due: DateTime.UtcNow.AddDays(rnd.Next(1, 7)).Date,
                expiration: DateTimeOffset.UtcNow.AddDays(rnd.Next(365)).Second,
                fine: (double)Math.Round((decimal)(10 * rnd.NextDouble()), 4),
                interest: (double)Math.Round((decimal)(10 * rnd.NextDouble()), 4)
                //tags: new List<string> { "scheduled" },
                //discounts: new List<Dictionary<string, object>>() {
                //    new Dictionary<string, object> {
                //        {"percentage", 10},
                //        {"due", new DateTime(2021, 4, 25)}
                //    }
                //}
            );
        }
    }
}
