using System.Text.Json.Serialization;

namespace Stark.Model.Config
{
    public class InvoiceWorkerConfig
    {
        public int IntervalHours { get; set; }

        public InvoiceClientConfig[] Clients { get; set; } = [ ];
    };
}
