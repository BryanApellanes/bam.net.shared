using Bam.Net.Server;
using Bam.Net.ServiceProxy;

namespace DefaultNamespace
{
    public delegate void ContentNotFoundEventHandler(IHttpResponder responder, IHttpContext context, string[] checkedPaths);
}