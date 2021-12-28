using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class GZipBase64Untransformer : IValueUntransformer<string, string>
    {
        public GZipBase64Untransformer()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public string Untransform(string base64EncodedZippedBytes)
        {
            byte[] zippedBytes = base64EncodedZippedBytes.FromBase64();
            byte[] unzippedBytes = zippedBytes.GUnzip();
            return Encoding.GetString(unzippedBytes);
        }

        public IValueTransformer<string, string> GetTransformer()
        {
            return new GZipBase64Transformer();
        }
    }
}
