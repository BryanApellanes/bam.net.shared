using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.PathHandlers
{
    public class DataInstancePath : DataPath
    {
        public DataInstancePath()
        {
            PathFormat = "{TypeName}/{Id}.{Format}";
        }

        public string Id { get; set; }
        public override string[] RequiredProperties
        {
            get
            {
                return new string[] { nameof(TypeName), nameof(Format), nameof(Id) };
            }
        }
    }
}
