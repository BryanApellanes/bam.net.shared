/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server
{
    /// <summary>
    /// BamServer where EnableDao is false
    /// and EnableServiceProxy is true
    /// </summary>
    public class BamAppServiceProxyServer: BamAppServer
    {
        public BamAppServiceProxyServer(BamConf conf)
            : base(conf)
        {
            this.EnableDao = false;
            this.EnableServiceProxy = true;
        }
    }
}
