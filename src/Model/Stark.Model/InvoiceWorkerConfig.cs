using Stark.API.Model.Invoice;
using System.Text.Json.Serialization;

namespace Stark.Model
{
    public record InvoiceWorkerConfig(
        [property: JsonPropertyName("IntervalHours")] int IntervalHours
    );
}
