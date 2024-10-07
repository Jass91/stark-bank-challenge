
namespace Stark.Application.Invoice
{
    public interface IInvoiceService
    {
        bool Proccess(StarkBank.Invoice invoiceLog);

        IEnumerable<StarkBank.Invoice> Create(IEnumerable<StarkBank.Invoice> invoices);
    }
}
