using Bam.Net.Presentation.Html;
using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.ServiceProxy
{
    public class MethodFormContextHandler : ResponderContextHandler<ServiceProxyResponder>
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override IHttpResponse HandleContext(IHttpContext request)
        {
            // Use InputFormProvider as basis of implementation
            //InputFormProvider inputFormProvider = new InputFormProvider();
            throw new NotImplementedException();
        }
    }
}
