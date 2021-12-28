using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class Base64Untransformer : IValueUntransformer<string, byte[]>
    {
        public byte[] Untransform(string encoded)
        {
            return encoded.FromBase64();
        }

        public IValueTransformer<byte[], string> GetTransformer()
        {
            return (IValueTransformer<byte[], string>)new Base64Transformer();
        }
    }
}
