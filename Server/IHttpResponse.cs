using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public interface IHttpResponse
    {
        void Send(IResponse response, int statusCode = 0);
    }
}
