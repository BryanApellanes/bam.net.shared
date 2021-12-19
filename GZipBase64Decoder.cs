using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipBase64Decoder : IValueDecoder<string, string>
    {
        public GZipBase64Decoder()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public string Decode(string base64EncodedZippedBytes)
        {
            byte[] zippedBytes = base64EncodedZippedBytes.FromBase64();
            byte[] unzippedBytes = zippedBytes.GUnzip();
            return Encoding.GetString(unzippedBytes);
        }

        public IValueEncoder<string, string> GetEncoder()
        {
            return new GZipBase64Encoder();
        }
    }
}
