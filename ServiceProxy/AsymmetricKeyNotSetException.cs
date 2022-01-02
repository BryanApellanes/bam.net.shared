using Bam.Net.ServiceProxy.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class AsymmetricKeyNotSetException : Exception
    {
        public AsymmetricKeyNotSetException(SecureChannelSession secureChannelSession) 
            : base($"Asymmetric Key Not Set: \r\n{secureChannelSession?.ToJson(true)}")
        {
        }
    }
}
