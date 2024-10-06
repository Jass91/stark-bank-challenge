namespace Stark.Model.Config
{
    public record StarkApiConfig
    (
        string Environment,
        string ProjectId,
        string PrivateKeyFilePath,
        string PublicKeyFilePath
    );
}
