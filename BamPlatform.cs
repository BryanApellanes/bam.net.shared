using Bam.Net.Analytics;
using Bam.Net.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net
{
    public class BamPlatform
    {
        static BamPlatform()
        {
            AppDomain.CurrentDomain.DomainUnload += async (o, a) => await StopServersAsync();
            Servers = new HashSet<IManagedServer>();
        }

        public static HashSet<IManagedServer> Servers { get; }
        
        internal static async Task<T> CreateManagedServerAsync<T>(Func<T> initializer) where T : IManagedServer
        {
            return await Task.Run(() =>
            {
                T server = initializer();
                Servers.Add(server);
                return server;
            });
        }

        /// <summary>
        /// Gets an integer value between 1024 and 65535 for the specified string.  Returns
        /// the same value for repeated calls with the same string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetUnprivilegedPortForName(string name)
        {
            return name.ToHashIntBetween(HashAlgorithms.SHA256, 1024, 65535);
        }

        public static async Task<BamServer> CreateServerAsync(string name)
        {
            return await CreateServerAsync(new ManagedServerHostBinding(name));
        }

        /// <summary>
        /// Create a BamServer that listens for requests to "localhost" on a random port from 8080 to 65535.
        /// </summary>
        /// <returns>BamServer</returns>
        public static async Task<BamServer> CreateServerAsync()
        {
            return await CreateServerAsync(RandomNumber.Between(8079, 65535));
        }

        /// <summary>
        /// Create a BamServer that listens for request to "localhost" on the specified port.
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static async Task<BamServer> CreateServerAsync(int port)
        {
            return await CreateServerAsync(new HostBinding(port));
        }

        /// <summary>
        /// Create a BamServer that listens for request on the specified HostBinding.
        /// </summary>
        /// <param name="hostBinding"></param>
        /// <returns></returns>
        public static async Task<BamServer> CreateServerAsync(HostBinding hostBinding)
        {
            return await Task.Run(() =>
            {
                BamServer bamServer = new BamServer(hostBinding);
                Servers.Add(bamServer);
                return bamServer;
            });
        }

        public static async Task StopServersAsync()
        {
            await Task.Run(() =>
            {
                Task.WaitAll(Servers.EachAsync(s => s.Stop()));
            });
        }
    }
}
