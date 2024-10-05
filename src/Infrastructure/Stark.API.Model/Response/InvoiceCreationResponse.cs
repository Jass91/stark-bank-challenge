using Stark.API.Model.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Stark.API.Model.Response
{
    public record InvoiceCreationResponse(
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("invoices")] List<InvoiceModel> Invoices
    );
}
