
namespace Stark.Application.Invoice
{
    public interface IInvoiceService
    {
        bool Process(StarkBank.Invoice invoiceLog);

        IEnumerable<StarkBank.Invoice> Create(IEnumerable<StarkBank.Invoice> invoices);
    }
}
