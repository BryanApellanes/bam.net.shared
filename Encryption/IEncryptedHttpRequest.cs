namespace Bam.Net.Encryption
{
    public interface IEncryptedHttpRequest : IHttpRequest
    {
        Cipher ContentCipher { get; }
    }
}