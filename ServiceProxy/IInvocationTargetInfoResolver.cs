using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy
{
    /// <summary>
    /// When implemented, resolves the class and method name the specified context is targeting for invocation.  Not to be confused 
    /// with the associated Type and MethodInfo, this method merely identifies them by name (string) for resolution later by a different mechanism.
    /// </summary>
    public interface IInvocationTargetInfoResolver
    {
        InvocationTargetInfo ResolveInvocationTarget(IHttpContext context);
    }
}
