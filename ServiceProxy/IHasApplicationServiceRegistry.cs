using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public interface IHasApplicationServiceRegistry
    {
        ApplicationServiceRegistry ApplicationServiceRegistry { get; set; }
    }
}
