using Bam.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.net
{
    public class JsonValueConverter<T> : EncodingValueConverter, IValueConverter<T>
    {
        public T ConvertBytesToObject(byte[] bytes)
        {
            string json = ConvertBytesToString(bytes);
            return json.FromJson<T>();
        }

        public byte[] ConvertObjectToBytes(T value)
        {
            string json = value.ToJson();
            return ConvertStringToBytes(json);
        }
    }
}
