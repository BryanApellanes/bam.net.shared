using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.PathHandlers
{
    public class DataQueryPath : DataPath
    {
        public DataQueryPath()
        {
            PathFormat = "{TypeName}.{Ext}?{QueryString}";
        }

        public string QueryString { get; set; }
        public override string[] RequiredProperties
        {
            get
            {
                return new string[] { "TypeName", "Ext", "QueryString" };
            }
        }
    }
}
