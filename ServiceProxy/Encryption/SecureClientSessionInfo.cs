using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.ServiceProxy.Encryption;

namespace Bam.Net.ServiceProxy.Encryption
{
    [Obsolete("Use SecureChannelSession instead")]
    public class SecureClientSessionInfo
    {
        public ClientSessionInfo ClientSessionInfo { get; set; }

        [Obsolete("Use SecureSessionId instead.")]
        public Cookie SecureSessionCookie { get; set; }
        public string SecureSessionId { get; set; }
        public string SessionKey { get; set; }
        public string SessionIV { get; set; }

        public override bool Equals(object obj)
        {
            SecureClientSessionInfo info = obj as SecureClientSessionInfo;
            if(info != null)
            {
                return info.ClientSessionInfo.Equals(ClientSessionInfo) && info.SecureSessionCookie.Equals(SecureSessionCookie) && info.SessionKey.Equals(SessionKey) && info.SessionIV.Equals(SessionIV);
            }
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return $"{ClientSessionInfo.ToString()}::SessionCookie={SecureSessionCookie.ToString()};SessionKey=XXX;SessionIV=XXX";
        }
    }
}
