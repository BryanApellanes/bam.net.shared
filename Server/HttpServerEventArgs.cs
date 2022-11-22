using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bam.Net.Server
{
    public class HttpServerEventArgs: EventArgs
    {
        public HostBinding[] HostBindings { get; set; }
        public string HostBindingString
        {
            get
            {
                return string.Join("\r\n", HostBindings.Select(hp => hp.ToString()).ToArray());
            }
        }
    }
}
