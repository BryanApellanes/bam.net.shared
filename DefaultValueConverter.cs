using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{ 
    public class DefaultValueConverter : IValueConverter
    {
        public virtual string ConvertBytesToString(byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(byte b in bytes)
            {
                stringBuilder.Append(b);
            }
            return stringBuilder.ToString();
        }

        public virtual byte[] ConvertStringToBytes(string value)
        {
            List<byte> bytes = new List<byte>();
            foreach(char c in value)
            {
                bytes.Add(Convert.ToByte(c));
            }
            return bytes.ToArray();
        }
    }
}
