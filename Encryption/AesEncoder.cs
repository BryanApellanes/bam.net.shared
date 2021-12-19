﻿using Bam.Net.ServiceProxy.Secure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class AesEncoder : ValueEncoder<string, byte[]>
    {
        public ClientSessionInfo ClientSessionInfo { get; set; }
        public override string Decode(byte[] cipherBytes)
        {
            return GetDecoder().Decode(cipherBytes);
        }

        public override byte[] Encode(string plainText)
        {
            return ClientSessionInfo.GetSymetricCipherBytes(plainText);
        }

        public override IValueDecoder<byte[], string> GetDecoder()
        {
            return new AesDecoder();
        }
    }
}
