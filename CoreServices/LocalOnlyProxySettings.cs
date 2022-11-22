using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.CoreServices
{
    public class LocalOnlyProxySettings : ProxySettings
    {
        public LocalOnlyProxySettings()
        {
        }

        public override ClientCodeSource ClientCodeSource 
        { 
            get => ClientCodeSource.Local; 
            set 
            {
                // always Local
            } 
        }
    }
}
