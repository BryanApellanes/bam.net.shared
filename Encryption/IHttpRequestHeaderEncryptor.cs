namespace Bam.Net.Encryption
{
    public interface IHttpRequestHeaderEncryptor
    {
        IEncryptor Encryptor { get; }

        void EncryptHeaders(IHttpRequest request);
    }
}