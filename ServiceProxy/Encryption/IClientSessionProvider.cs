using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    public interface IClientSessionProvider
    {
        Task<ClientSessionInfo> RetrieveClientSessionAsync(string sessionIdentifier);

        Task<ClientSessionInfo> StartClientSessionAsync(Instant clientNow);
    }
}