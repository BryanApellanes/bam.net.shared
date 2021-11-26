using Bam.Net.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server.PathHandlers
{
    public abstract class DataPath : IHasRequiredProperties
    {
        // /{Type}.{ext}?{Query}
        // /{Type}/{Id}.{ext}
        // /{Type}/{Id}/{ChildListProperty}.{Format} 
        public string PathFormat { get; protected set; }
        public string TypeName { get; set; }
        public string Format { get; set; }
        public abstract string[] RequiredProperties { get; }
        
        public bool IsValid()
        {
            bool allGood = true;
            foreach (string property in RequiredProperties)
            {
                if (!allGood)
                {
                    break;
                }
                allGood = !string.IsNullOrEmpty((string)this.Property(property));
            }
            return allGood;
        }
    }
}
