/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.ServiceProxy;

namespace Bam.Net.Server
{
    public interface IHttpContextHandler
    {
        event EventHandler HandleContextStarted;
        event EventHandler HandleContextCompleted;
        event EventHandler HandleContextExceptionThrown;

        Task<IHttpResponse> HandleContextAsync(IHttpContext context);
    }
}
