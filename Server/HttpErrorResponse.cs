using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class HttpErrorResponse : HttpResponse
    {
        public HttpErrorResponse(Exception ex)
        {
            this.Exception = ex;
        }

        public Exception Exception { get; }

        protected override byte[] GetOutput()
        {
            this.Content = $"{Exception?.GetType().Name}: {Exception.Message}";
            return base.GetOutput();
        }
    }
}
