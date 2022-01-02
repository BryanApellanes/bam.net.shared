using Bam.Net.ServiceProxy.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class SessionKeyNotSetException : Exception
    {
        public SessionKeyNotSetException(SecureChannelSession secureChannelSession) 
            : base($"Session Key Not Set: \r\n{secureChannelSession?.ToJson(true)}")
        { 
        }
    }
}
