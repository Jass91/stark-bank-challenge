using Stark.API.Model.Enums;

namespace Stark.API.Model.Config
{
    public class StarkApiConfig
    {
        public StarkEnvironment Environment { get; set; }
        
        public string? ProjectId { get; set; }

        public required string PrivateKeyFilePath { get; set; }

        public string AccessId =>  $"project/{ProjectId}";

        public StarkApiConfig()
        {
            // pode fazer certas validacoes aqui
        }
    }
}
