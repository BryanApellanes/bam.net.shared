using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface ICommunicationKeySet
    {
        string ServerHostName { get; set; }
        string ClientHostName { get; set; }
    }
}
