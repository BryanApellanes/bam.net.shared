using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Secure
{
    public class SecureChannelException : Exception
    {
        public SecureChannelException(string message): base(message)
        {
        }

        public SecureChannelException(SecureChannelMessage secureChannelMessage) : base(secureChannelMessage.Message)
        { 
        }
    }
}
