/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class EncryptedValidationTokenNotFoundException: Exception 
    {
        public EncryptedValidationTokenNotFoundException(string message)
            : base(message)
        { }
    }
}
