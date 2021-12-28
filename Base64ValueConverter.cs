using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class Base64ValueConverter : IValueConverter
    {
        public string ConvertBytesToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public byte[] ConvertStringToBytes(string value)
        {
            return Convert.FromBase64String(value);
        }
    }
}
