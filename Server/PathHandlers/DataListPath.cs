using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.PathHandlers
{
    public class DataListPath : DataPath
    {
        public DataListPath()
        {
            PathFormat = "{TypeName}/{Id}/{ChildListProperty}.{Ext}";
        }
        public string Id { get; set; }
        public string ChildListProperty { get; set; }
        public override string[] RequiredProperties
        {
            get
            {
                return new string[] { nameof(TypeName), nameof(Format), nameof(Id), nameof(ChildListProperty) };
            }
        }
    }
}
