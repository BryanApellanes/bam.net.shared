using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaEncoder : Encoder
    {
        public override T Decode<T>(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override byte[] Encode(object value)
        {
            throw new NotImplementedException();
        }
    }
}
