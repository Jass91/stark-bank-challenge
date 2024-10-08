using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stark.Workers;

namespace Stark.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        private readonly IInvoiceGenerator _invoiceWorker;

        public WorkerController(IInvoiceGenerator invoiceWorker)
        {
            _invoiceWorker = invoiceWorker;
        }

        /// <summary>
        /// Allow admin to start worker if it is stopped
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("invoice/start")]
        public async Task<OkObjectResult> StartWorker(CancellationToken cancellationToken)
        {
            if (_invoiceWorker.Running)
                return Ok("Worker is already running");

            await _invoiceWorker.StartAsync(cancellationToken);

            return Ok("Worker is started");
        }

        /// <summary>
        /// Allow to get worker status
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("invoice/status")]
        public OkObjectResult WorkerStatus()
        {
            return Ok(_invoiceWorker.Running ? "Running" : "Stopped");
        }

        /// <summary>
        /// Allow to get worker execution number
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("invoice/execution")]
        public OkObjectResult WorkerExecution()
        {
            return Ok(_invoiceWorker.CurrentExecution);
        }
    }
}
