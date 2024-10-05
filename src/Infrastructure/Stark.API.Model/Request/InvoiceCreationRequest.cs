using Stark.API.Model.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Stark.API.Model.Request
{
    public record InvoiceCreationRequest
    (
        [property: JsonPropertyName("invoices")] IEnumerable<InvoiceModel> Invoices
    );
}
