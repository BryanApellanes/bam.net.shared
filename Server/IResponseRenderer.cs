/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bam.Net.Server.Renderers;
using Bam.Net.Presentation;
using Bam.Net.Server.ServiceProxy;

namespace Bam.Net.Server
{
    public interface IRequestRenderer: IWebRenderer
    {
        ServiceProxyInvocation Request { get; set; }
    }
}
