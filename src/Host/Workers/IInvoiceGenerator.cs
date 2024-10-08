namespace Stark.Workers
{
    public interface IInvoiceGenerator : IHostedService
    {
        bool Running { get; }

        int CurrentExecution { get; }
    }
}
