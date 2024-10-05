using System.Text.Json.Serialization;

namespace Stark.API.Model.Invoice
{
    public record InvoiceDescription(
        [property: JsonPropertyName("key")] string Key,
        [property: JsonPropertyName("value")] string Value
    );

}
