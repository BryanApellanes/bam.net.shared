/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Encryption
{
    /// <summary>
    /// Denotes a class that requires encryption when streamed to file
    /// or network.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EncryptAttribute : Attribute
    {
        public EncryptAttribute()
        {
            this.EncryptionScheme = EncryptionSchemes.Symmetric;
        }

        public EncryptionSchemes EncryptionScheme { get; set; }
    }
}
