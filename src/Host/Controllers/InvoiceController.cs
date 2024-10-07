using StarkBank;
using StarkBank.Error;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Stark.Application.Invoice;

namespace Stark.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly Project _project;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;
        
        public InvoiceController
        (
            Project project,
            IInvoiceService invoiceService,
            ILogger<InvoiceController> logger
        )
        {
            _logger = logger;
            _project = project;
            _invoiceService = invoiceService;
        }

        // TODO: remover ou deixar pra teste
        [HttpPost]
        public ActionResult<IEnumerable<Invoice>> Create([FromBody] List<StarkBank.Invoice> invoices)
        {
            var createdInvoices = _invoiceService.Create(invoices);

            return Ok(createdInvoices);
        }

        [HttpPost("proccess")]
        public ActionResult<Invoice> Proccess([FromBody] JsonElement evt)
        {
            try
            {
                var signature = Request.Headers["digital-signature"].ToString();

                Event parsedEvent = Event.Parse(
                    content: evt.GetRawText(),
                    user: _project,
                    signature: signature
                );

                if (parsedEvent.Subscription == "invoice" && parsedEvent.Log is Invoice.Log invoiceLog)
                {
                    if (invoiceLog.Type == "paid")
                    {
                        var isSuccess = _invoiceService.Proccess(invoiceLog.Invoice);
                        if (isSuccess)
                            return Ok();
                    }
                }

                return Ok();
            }
            catch (InvalidSignatureError)
            {
                return new ForbidResult();
            }
        }
    }
}
