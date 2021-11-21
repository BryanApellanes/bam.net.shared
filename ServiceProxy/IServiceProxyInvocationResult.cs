using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public interface IServiceProxyInvocationResult
    {
        object Result { get; set; }
        Exception Exception { get; set; }
        bool Success { get; set; }
    }
}
