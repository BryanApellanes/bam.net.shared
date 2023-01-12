using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class ManagedServerHostBinding : HostBinding
    {
        public ManagedServerHostBinding(IManagedServer server)
        {
            this.Server = server;
        }

        /// <summary>
        /// Create an instance of ManagedServerHostBinding using the specified server name.
        /// </summary>
        /// <param name="serverName"></param>
        public ManagedServerHostBinding(string serverName)
        {
            this.ServerName = serverName;
        }

        IManagedServer _server;
        protected internal IManagedServer Server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
                Port = BamPlatform.GetUnprivilegedPortForName(value.ServerName);
            }
        }

        string _serverName;
        /// <summary>
        /// Gets or sets the server name.  Not to be confused with the hostname, the ServerName is an identifier assigned to the server for programatic reference.
        /// </summary>
        public string ServerName
        {
            get
            {
                return Server?.ServerName ?? _serverName;
            }
            set
            {
                _serverName = value;
            }
        }

        public override int Port
        {
            get
            {
                return BamPlatform.GetUnprivilegedPortForName(ServerName);
            }
            set
            {
                // always use derived port
            }
        }
    }
}
