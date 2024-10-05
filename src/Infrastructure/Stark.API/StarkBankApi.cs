using System.Text;
using System.Text.Json;
using Stark.API.Model.Invoice;
using Stark.API.Model.Config;
using NBitcoin;
using NBitcoin.Crypto;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using System.Numerics;
using Stark.API.Model.Response;
using Stark.API.Model.Request;
using System.Text.Json.Serialization;

namespace Stark.API
{
    public class StarkBankApi : BaseStarkBankApi, IStarkBankApi
    {
        public StarkBankApi(StarkApiConfig config, HttpClient httpClient) : base(config, httpClient)
        {
         
        }

        public async Task<InvoiceCreationResponse> CreateInvoices(IEnumerable<InvoiceModel> invoices)
        {
            var payload = Serialize(new InvoiceCreationRequest(invoices));

            AddAuthHeaders(payload);

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync($"{ApiVersion}/invoice", content);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<InvoiceCreationResponse>(responseData)!;
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error creating invoices: {errorResponse}");
            }
        }
    }

}
