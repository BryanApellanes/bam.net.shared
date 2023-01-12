using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public class JsonResponseConverter : IResponseConverter
    {
        public T ConvertResponse<T>(HttpClientResponse clientResponse)
        {
            return clientResponse.Content.FromJson<T>();
        }
    }
}
