namespace Stark.Application.Invoice
{
    public interface IInvoiceService
    {
        Task<IEnumerable<API.Model.Invoice.InvoiceModel>> Create(IEnumerable<API.Model.Invoice.InvoiceModel> invoices);
    }
}
