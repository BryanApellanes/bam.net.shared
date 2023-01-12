using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public interface IManagedServer
    {
        string ServerName { get; }
        HostBinding DefaultHostBinding { get; }
        void Start();
        void Stop();

        void TryStop();
    }
}
