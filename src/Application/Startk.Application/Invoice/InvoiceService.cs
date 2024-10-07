
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace Stark.Application.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly StarkBank.Project _project;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(StarkBank.Project project, ILogger<InvoiceService> logger)
        {
            _logger = logger;
            _project = project;
        }

        public IEnumerable<StarkBank.Invoice> Create(IEnumerable<StarkBank.Invoice> invoices)
        {
            return StarkBank.Invoice.Create(invoices.ToList(), _project);
        }

        /*
            Receives the webhook callback of the Invoice credit and sends the received amount
            (minus eventual fees) to the following account using a Transfer:
            a.bank code: 20018183
            b.branch: 0001
            c.account: 6341320293482496
            d.name: Stark Bank S.A.
            e.tax ID: 20.018.183 / 0001 - 80
            f.account type: payment
        */
        public bool Proccess(StarkBank.Invoice invoice)
        {
            try
            {
                var branchCode = "0001";
                var bankCode = "20018183";
                var accountType = "payment";
                var name = "Stark Bank S.A.";
                var taxId = "20.018.183/0001-80";
                var accountNumber = "6341320293482496";

                // Calcular o montante líquido
                var netAmount = CalculateNetAmount(invoice);

                _logger.LogInformation($"Net amount: {netAmount} cents");

                // Realizar a transferência
                var transfer = StarkBank.Transfer.Create
                (
                    user: _project,
                    transfers: [
                        new StarkBank.Transfer(netAmount, name, taxId, bankCode, branchCode, accountNumber, accountType)
                    ]
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to proccess invoice {InvoiceID}", invoice.ID);
                return false;
            }
        }

        private long CalculateNetAmount(StarkBank.Invoice invoice)
        {
            // Convertendo para decimal para maior precisão em cálculos financeiros
            decimal amount = invoice.Amount; // Valor total da fatura em centavos

            // Taxa fixa (pode ser nula, então usamos 0 como padrão)
            decimal fee = invoice.Fee ?? 0;

            // Multa em percentual (pode ser nula, então usamos 0 como padrão)
            decimal finePercent = Convert.ToDecimal(invoice.Fine ?? 0);

            // Juros em percentual (pode ser nula, então usamos 0 como padrão)
            decimal interestPercent = Convert.ToDecimal(invoice.Interest ?? 0);

            // Valores fixos de multa e juros (se disponíveis)
            decimal fineAmount = invoice.FineAmount ?? 0;
            decimal interestAmount = invoice.InterestAmount ?? 0;

            // Se FineAmount é zero e FinePercent > 0, calcular a multa com base no percentual
            if (fineAmount == 0 && finePercent > 0)
            {
                fineAmount = Math.Round(amount * (finePercent / 100), 4);
                _logger.LogInformation("Fine amount calculated: {FineAmount} cents", fineAmount);
            }

            // Se InterestAmount é zero e InterestPercent > 0, calcular os juros com base no percentual
            if (interestAmount == 0 && interestPercent > 0)
            {
                interestAmount = Math.Round(amount * (interestPercent / 100), 0, MidpointRounding.AwayFromZero);
                _logger.LogInformation("Interest Calculated: {InterestAmount} cents", interestAmount);
            }

            // Calculando o total das deduções
            decimal totalDeductions = fee + fineAmount + interestAmount;
            _logger.LogInformation("Total Deductions (Fee: {Fee}, FineAmount: {FineAmount}, InterestAmount: {InterestAmount}): {TotalDeductions} cents", fee, fineAmount, interestAmount, totalDeductions);

            // Calculando o montante líquido
            decimal netAmount = amount - totalDeductions;
            _logger.LogInformation("Net Amount: {NetAmount} cents", netAmount);

            // Arredondando para garantir que esteja em centavos (sem casas decimais)
            return (long)Math.Round(netAmount, 4);
        }

    }
}
