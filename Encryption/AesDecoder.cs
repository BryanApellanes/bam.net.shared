﻿using Bam.Net;
using Bam.Net.Server.ServiceProxy.Data;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesDecoder : IValueDecoder<byte[], string>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public AesDecoder()
        {
            this.AesEncoder = new AesEncoder() { AesDecoder = this };
        }

        public AesEncoder AesEncoder { get; internal set; }

        [Inject]
        public ISecureChannelSessionManager SecureChannelSessionManager { get; set; }

        public IHttpContext HttpContext { get; set; }
        public object Clone()
        {
            object clone = new AesDecoder() { AesEncoder = AesEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            AesDecoder clone = new AesDecoder();
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public string Decode(byte[] cipherBytes)
        {
            SecureChannelSession session = SecureChannelSessionManager.GetSecureChannelSessionForContext(HttpContext);

            ClientSessionInfo clientSessionInfo = session.ToClientSessionInfo();
            return clientSessionInfo.GetPlainText(cipherBytes);
        }

        public IValueEncoder<string, byte[]> GetEncoder()
        {
            return this.AesEncoder;
        }
    }
}
