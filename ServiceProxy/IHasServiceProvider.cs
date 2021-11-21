using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.CoreServices;
using Bam.Net.Incubation;

namespace Bam.Net.ServiceProxy
{
    // TODO: rename this to IHasServiceRegistry
    public interface IHasServiceProvider
    {
        ServiceRegistry ServiceProvider { get; set; }
    }
}
