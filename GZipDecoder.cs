using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipDecoder : IValueDecoder<byte[], string>
    {
        public Encoding Encoding { get; set; }
        public string Decode(byte[] encoded)
        {
            byte[] unzipped = encoded.GUnzip();
            return Encoding.GetString(unzipped);
        }

        public IValueEncoder<string, byte[]> GetEncoder()
        {
            return new GZipEncoder();
        }
    }
}
