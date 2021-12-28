using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipUntransformer : IValueUntransformer<byte[], string>
    {
        public Encoding Encoding { get; set; }
        public string Untransform(byte[] encoded)
        {
            byte[] unzipped = encoded.GUnzip();
            return Encoding.GetString(unzipped);
        }

        public IValueTransformer<string, byte[]> GetTransformer()
        {
            return new GZipTransformer();
        }
    }
}
