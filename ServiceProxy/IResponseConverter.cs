using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public interface IResponseConverter
    {
        T ConvertResponse<T>(HttpClientResponse clientResponse);
    }
}
