using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface ICommunicationKeySet
    {
        /// <summary>
        /// Gets or sets the server host name.
        /// </summary>
        string ServerHostName { get; set; }

        /// <summary>
        /// Gets or sets the client host name.
        /// </summary>
        string ClientHostName { get; set; }
    }
}
