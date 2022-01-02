using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public interface IApplicationKeySet :IKeySet
    {
        string ApplicationName { get; set; }
    }
}
