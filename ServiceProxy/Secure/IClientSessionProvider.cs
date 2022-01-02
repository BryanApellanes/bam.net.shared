using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Secure
{
    public interface IClientSessionProvider
    {
        Task<ClientSession> RetrieveClientSessionAsync(string sessionIdentifier);

        Task<ClientSession> StartClientSessionAsync(Instant clientNow);
    }
}