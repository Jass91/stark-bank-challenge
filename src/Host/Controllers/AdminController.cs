using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stark.Workers;

namespace Stark.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IInvoiceGenerator _invoiceWorker;

        public AdminController(IInvoiceGenerator invoiceWorker)
        {
            _invoiceWorker = invoiceWorker;
        }

        /// <summary>
        /// Allows admin to start worker if it is stopped
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("worker/invoice/start")]
        public async Task<OkObjectResult> StartWorker(CancellationToken cancellationToken)
        {
            if (_invoiceWorker.Running)
                return Ok("Worker is already running");

            await _invoiceWorker.StartAsync(cancellationToken);

            return Ok("Worker is started");
        }
    }
}
