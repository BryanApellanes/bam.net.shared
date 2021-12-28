using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class BsonValueConverter<TData> : IValueConverter<TData>
    {
        /// <summary>
        /// Converts the specified byte array into an instance of TData
        /// by deserializing it as bson.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public TData ConvertBytesToObject(byte[] bytes)
        {
            return bytes.FromBson<TData>();
        }

        /// <summary>
        /// Converts the specified bson byte array to the equivalent json string.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string ConvertBytesToString(byte[] bytes)
        {
            return bytes.FromBson<TData>().ToJson();
        }

        /// <summary>
        /// Converts the specified value to bson.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] ConvertObjectToBytes(TData value)
        {
            return value.ToBson();
        }

        /// <summary>
        /// Converts the specified json string to an instance of TData then
        /// converts to the equivalent bson representation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] ConvertStringToBytes(string json)
        {
            return json.FromJson<TData>().ToBson();
        }
    }
}
