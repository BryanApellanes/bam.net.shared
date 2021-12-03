using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public abstract class Encoder : IEncoder
    {
        public Encoder(Encoding encoding = null)
        {
            this.Encoding = encoding ?? Encoding.UTF8;
        }

        public Encoding Encoding
        {
            get;
        }

        public T Decode<T>(string base64EncodedString)
        {
            byte[] bytes = base64EncodedString.FromBase64();
            return Decode<T>(bytes);
        }

        public abstract T Decode<T>(byte[] bytes);

        public abstract byte[] Encode(object value);
    }
}
