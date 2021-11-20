using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class HandleRequestErrorResult : IHandleRequestResult
    {
        public Exception Exception { get; set; }

        public void SendResponse(IResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
