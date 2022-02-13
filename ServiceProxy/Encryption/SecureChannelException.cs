using Bam.Net.ServiceProxy.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy.Encryption
{
    public class SecureChannelException : Exception
    {
        public SecureChannelException(string message): base(message)
        {
        }

        public SecureChannelException(SecureChannelResponseMessage secureChannelMessage) : base(secureChannelMessage.Message)
        { 
        }
    }
}
