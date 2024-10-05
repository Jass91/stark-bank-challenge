
using Stark.Application.Invoice;
using Stark.Model;

namespace Stark.Application
{
    public class InvoiceGenerator : IHostedService, IDisposable
    {
        private Timer? _timer = null;
        private int executionCount = 0;
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
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Invoice worker running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(_invoiceWorkerConfig.IntervalHours));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
        }
    }
}
