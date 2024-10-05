using System.Text.Json.Serialization;

namespace Stark.API.Model.Invoice
{
    public record InvoiceModel(
     [property: JsonPropertyName("amount")] int? Amount = null,
     [property: JsonPropertyName("brcode")] string? Brcode = null,
     [property: JsonPropertyName("created")] DateTime? Created = null,
     [property: JsonPropertyName("descriptions")] List<InvoiceDescription>? Descriptions = null,
     [property: JsonPropertyName("discountAmount")] int? DiscountAmount = null,
     [property: JsonPropertyName("discounts")] List<object>? Discounts = null,
     [property: JsonPropertyName("displayDescription")] string? DisplayDescription = null,
     [property: JsonPropertyName("due")] DateTimeOffset? Due = null,
     [property: JsonPropertyName("expiration")] int? Expiration = null,
     [property: JsonPropertyName("fee")] int? Fee = null,
     [property: JsonPropertyName("fine")] decimal? Fine = null,
     [property: JsonPropertyName("fineAmount")] int? FineAmount = null,
     [property: JsonPropertyName("id")] string? Id = null,
     [property: JsonPropertyName("interest")] decimal? Interest = null,
     [property: JsonPropertyName("interestAmount")] int? InterestAmount = null,
     [property: JsonPropertyName("link")] string? Link = null,
     [property: JsonPropertyName("name")] string? Name = null,
     [property: JsonPropertyName("nominalAmount")] int? NominalAmount = null,
     [property: JsonPropertyName("pdf")] string? Pdf = null,
     [property: JsonPropertyName("rules")] List<object>? Rules = null,
     [property: JsonPropertyName("splits")] List<object>? Splits = null,
     [property: JsonPropertyName("status")] string? Status = null,
     [property: JsonPropertyName("tags")] List<object>? Tags = null,
     [property: JsonPropertyName("taxId")] string? TaxId = null,
     [property: JsonPropertyName("transactionIds")] List<object>? TransactionIds = null,
     [property: JsonPropertyName("updated")] DateTime? Updated = null
 );
}
