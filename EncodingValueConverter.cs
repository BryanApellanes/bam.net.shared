using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class EncodingValueConverter : IValueConverter
    {
        public EncodingValueConverter()
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public virtual string ConvertBytesToString(byte[] bytes)
        {
            return Encoding.GetString(bytes);
        }

        public virtual byte[] ConvertStringToBytes(string value)
        {
            return Encoding.GetBytes(value);
        }
    }
}
