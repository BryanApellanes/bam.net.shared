using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public interface IManagedServer
    {
        HostBinding DefaultHostBinding { get; }
        void Start();
        void Stop();
    }
}
