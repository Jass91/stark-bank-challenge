using System.Text;
using Stark.API.Model.Config;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using Stark.API.Model.Invoice;
using System.Text.Json;
using Stark.API.Model.Request;
using System.Text.Json.Serialization;

namespace Stark.API
{
    public abstract class BaseStarkBankApi
    {
        private readonly string _privateKey;
        private const string _apiVersion = "v2";
        private readonly StarkApiConfig _config;
        private readonly HttpClient _httpClient;

        protected HttpClient HttpClient => _httpClient;
        
        protected StarkApiConfig ApiConfig => _config;

        protected string PrivateKey => _privateKey;

        protected string ApiVersion => _apiVersion;

        protected BaseStarkBankApi(StarkApiConfig config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _privateKey = ReadPrivateKey(config.PrivateKeyFilePath);
        }

        protected long CurrentUnixTime => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        // Função que permite assinar a mensagem (curvas elipticas SECP256k1)
        protected string Sign(string message)
        {
            // Converte a mensagem para bytes
            var messageBytes = Encoding.UTF8.GetBytes(message);

            // Gera o hash SHA256 da mensagem
            var messageHash = SHA256.HashData(messageBytes);

            // Extrai a chave privada usando BouncyCastle
            var privateKeyParams = ExtractPrivateKeyFromPem(_privateKey);

            // Inicializa o assinador ECDSA com BouncyCastle (determinístico conforme RFC 6979)
            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            signer.Init(true, privateKeyParams);

            // Assina o hash
            var signatureComponents = signer.GenerateSignature(messageHash);
            var r = signatureComponents[0];
            var s = signatureComponents[1];

            // Converte para DER
            var derSignature = new DerSequence(new DerInteger(r), new DerInteger(s));
            var derSignatureBytes = derSignature.GetEncoded();


            // Retorna a assinatura em Base64
            var signatureBase64 = Convert.ToBase64String(derSignatureBytes);

            return signatureBase64;
        }

        protected void AddAuthHeaders(string payload)
        {
            // Criação da string de assinatura
            var accessId = ApiConfig.AccessId;
            var accessTime = CurrentUnixTime;

            // Cria a mensagem para assinatura
            var message = $"{accessId}:{accessTime}:";
            if (!string.IsNullOrEmpty(payload))
                message += payload;

            var signature = Sign(message);

            _httpClient.DefaultRequestHeaders.Add("Access-Signature", signature);
            _httpClient.DefaultRequestHeaders.Add("Access-Time", accessTime.ToString());
        }

        protected string Serialize<T>(T payload)
        {
            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Ignora campos com valor null
            });
        }

        // Função para extrair a chave privada do PEM usando BouncyCastle
        private ECPrivateKeyParameters ExtractPrivateKeyFromPem(string pem)
        {
            var pemReader = new PemReader(new StringReader(pem));
            var keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)pemReader.ReadObject();
            var privateKeyParams = (ECPrivateKeyParameters)keyPair.Private;
            return privateKeyParams;
        }

        private string ReadPrivateKey(string path)
        {
            using (var f = File.OpenRead(path))
            using (var sr = new StreamReader(f))
                return sr.ReadToEnd();
        }

    }

}
