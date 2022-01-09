using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipBase64Transformer : ValueTransformer<string, string>
    {
        public GZipBase64Transformer()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public override string ReverseTransform(string base64GZipData)
        {
            return GetReverseTransformer().ReverseTransform(base64GZipData);
        }

        public override string Transform(string inputText)
        {
            byte[] zippedBytes = inputText.GZip(Encoding);
            return Convert.ToBase64String(zippedBytes);
        }

        public override IValueReverseTransformer<string, string> GetReverseTransformer()
        {
            return new GZipBase64ReverseTransformer();
        }
    }
}
