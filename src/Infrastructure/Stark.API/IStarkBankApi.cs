using Stark.API.Model.Invoice;
using Stark.API.Model.Response;

namespace Stark.API
{
    public interface IStarkBankApi
    {
        Task<InvoiceCreationResponse> CreateInvoices(IEnumerable<InvoiceModel> invoices);
    }

}
