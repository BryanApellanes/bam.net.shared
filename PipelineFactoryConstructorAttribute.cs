using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class PipelineFactoryConstructorAttribute : Attribute
    {
    }
}
