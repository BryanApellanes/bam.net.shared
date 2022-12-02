using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class JsonResponseConverter : IResponseConverter
    {
        public T ConvertResponse<T>(HttpClientResponse clientResponse)
        {
            return clientResponse.Content.FromJson<T>();
        }
    }
}
