﻿using Bam.Net.Incubation;
using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    public interface IServiceProxyInvocationResolver: IInvocationTargetInfoResolver
    {
        ServiceProxyInvocation ResolveInvocationRequest(IHttpContext context, Incubator serviceProvider, params ProxyAlias[] aliases);
    }
}
