using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using Bam.Net.Services;

namespace Bam.Net.ServiceProxy
{
    public interface IHasWebServiceRegistry
    {
        WebServiceRegistry WebServiceRegistry { get; set; }
    }
}
