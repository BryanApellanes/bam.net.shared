using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MediaTypeAttribute : Attribute
    {
        public MediaTypeAttribute(string mediaType)
        {
            this.MediaType = mediaType;
        }

        public string MediaType { get; set; }
    }
}
