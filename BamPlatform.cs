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

        public static async Task<BamServer> CreateServerAsync()
        {
            return await CreateServerAsync(RandomNumber.Between(8079, 65535));
        }

        public static async Task<BamServer> CreateServerAsync(int port)
        {
            return await CreateServerAsync(new HostBinding(port));
        }

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
