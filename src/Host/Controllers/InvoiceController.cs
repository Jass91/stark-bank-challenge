using StarkBank;
using StarkBank.Error;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Stark.Application.Invoice;
using Microsoft.Extensions.Logging;

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

        [HttpPost("process")]
        public ActionResult<Invoice> Process([FromBody] JsonElement evt)
        {
            try
            {
                var content = evt.GetRawText();

                _logger.LogInformation("New Webhook received: {Event}", content);

                var signature = Request.Headers["digital-signature"].ToString();

                Event parsedEvent = Event.Parse(
                    user: _project,
                    content: content,
                    signature: signature
                );

                if (parsedEvent.Subscription == "invoice" && parsedEvent.Log is Invoice.Log invoiceLog)
                {

                    if (invoiceLog.Type == "paid")
                    {
                        _logger.LogInformation("Start process paid invoice {ID}", invoiceLog.Invoice.ID);

                        var isSuccess = _invoiceService.Process(invoiceLog.Invoice);
                        if (isSuccess)
                            _logger.LogInformation("Invoice {ID} proccessed", invoiceLog.Invoice.ID);
                    }
                }

                return Ok();
            }
            catch (InvalidSignatureError e)
            {
                _logger.LogError(e, "Error processing event");

                return new ForbidResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event");

                return StatusCode(500, ex.Message);
            }
        }
    }
}
