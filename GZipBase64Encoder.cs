using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipBase64Encoder : ValueEncoder<string, string>
    {
        public GZipBase64Encoder()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public override string Decode(string base64GZipData)
        {
            return GetDecoder().Decode(base64GZipData);
        }

        public override string Encode(string inputText)
        {
            byte[] zippedBytes = inputText.GZip(Encoding);
            return Convert.ToBase64String(zippedBytes);
        }

        public override IValueDecoder<string, string> GetDecoder()
        {
            return new GZipBase64Decoder();
        }
    }
}
