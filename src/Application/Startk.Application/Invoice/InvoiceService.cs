using Stark.API;
using Stark.API.Model.Invoice;
using Stark.API.Model.Response;

namespace Stark.Application.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IStarkBankApi _starkBankApi;   
        
        public InvoiceService(IStarkBankApi starkBankApi)
        {
            _starkBankApi = starkBankApi;    
        }

        public async Task<IEnumerable<InvoiceModel>> Create(IEnumerable<API.Model.Invoice.InvoiceModel> invoices)
        {
            var response = await _starkBankApi.CreateInvoices(invoices);

            return response.Invoices;
        }
    }
}
