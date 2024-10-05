using Microsoft.AspNetCore.Mvc;
using Stark.API.Model.Invoice;
using Stark.Application.Invoice;

namespace Stark.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {

        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _logger = logger;
            _invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<InvoiceModel>>> CreateInvoice([FromBody] IEnumerable<InvoiceModel> invoices)
        {
            var createdInvoices = await _invoiceService.Create(invoices);

            return Ok(createdInvoices);
        }
    }
}
