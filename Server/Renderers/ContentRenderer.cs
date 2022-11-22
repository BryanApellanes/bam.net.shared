/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bam.Net.Web;
using Bam.Net.Presentation.Html;
using System.Reflection;
using Bam.Net.Server.ServiceProxy;

namespace Bam.Net.Server.Renderers
{
    public abstract class ContentRenderer: WebRenderer
    {
        public ContentRenderer(ServiceProxyInvocation invocation, ContentResponder content, string contentType, params string[] extensions)
            :base(contentType, extensions)
        {
            this.ServiceProxyInvocation = invocation;
            this.ContentResponder = content;
        }

        protected ServiceProxyInvocation ServiceProxyInvocation
        {
            get;
            set;
        }
    }
}
