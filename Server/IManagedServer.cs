using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public interface IManagedServer
    {
        void Start();
        void Stop();
    }
}
