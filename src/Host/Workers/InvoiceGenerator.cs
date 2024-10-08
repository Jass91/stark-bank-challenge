
using Stark.Model.Config;
using Stark.Application.Invoice;

namespace Stark.Workers
{
    public class InvoiceGenerator : BackgroundService, IInvoiceGenerator
    {
        private int _currentExecution = 0;
        private bool _running = false;
        private PeriodicTimer? _timer;
        private readonly object _locker = new object();
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceGenerator> _logger;
        private readonly InvoiceWorkerConfig _invoiceWorkerConfig;

        public bool Running => _running;

        public int CurrentExecution => GetCurrentExecution();

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
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            _running = false;
            _timer?.Dispose();
            SetCurrentExecution(0);

            _logger.LogInformation("Invoice Generator is stopped");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _running = true;
            
            SetCurrentExecution(0);
            
            //_timer = new PeriodicTimer(TimeSpan.FromHours(_invoiceWorkerConfig.IntervalHours));
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            _logger.LogInformation("Invoice worker running.");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var currentExecution = GetCurrentExecution();

                // roda a cada 3 horas durante 24h, entao, precisa executar 8 vezes (8 * 3 = 24)
                if (currentExecution > 7)
                {
                    _logger.LogInformation("24 hours cycle completed.");
                    break;
                }

                _logger.LogInformation("Creating Invoices for execution #{Execution}...", currentExecution + 1);

                try
                {
                    CreateInvoices();
                    SetCurrentExecution(currentExecution + 1);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error trying to create invoices");

                    SetCurrentExecution(currentExecution - 1);
                }


            } while (await _timer!.WaitForNextTickAsync(stoppingToken));

            await StopAsync(stoppingToken);
        }


        private void CreateInvoices()
        {
            var rnd = new Random(DateTime.UtcNow.Second);
            
            // 0 ou 1
            var idx = rnd.Next(0, 2); 
            
            var max = ((int[])[8, 12])[idx];
            var invoices = new List<StarkBank.Invoice>();
            foreach (var i in Enumerable.Range(1, max))
                invoices.Add(MockInvoiceModel(rnd));

            _invoiceService.Create(invoices);
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

        private int GetCurrentExecution()
        {
            lock (_locker) { return _currentExecution; }
        }

        private void SetCurrentExecution(int value)
        {
            lock (_locker)
            {
                _currentExecution = value;
            }
        }
    }
}
