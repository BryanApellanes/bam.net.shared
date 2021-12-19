using Bam.net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public abstract class ValueEncoder<TInput, TOutput> : IValueEncoder<TInput, TOutput>, IValueConverter<TOutput>
    {
        public ValueEncoder()
        {
            this.ValueConverter = new JsonValueConverter<TOutput>();
        }

        public abstract TOutput Encode(TInput input);

        public abstract TInput Decode(TOutput output);

        public abstract IValueDecoder<TOutput, TInput> GetDecoder();

        public IValueConverter<TOutput> ValueConverter { get; set; }

        public TOutput ConvertBytesToObject(byte[] bytes)
        {
            return ValueConverter.ConvertBytesToObject(bytes);
        }

        public byte[] ConvertObjectToBytes(TOutput value)
        {
            return ValueConverter.ConvertObjectToBytes(value);
        }

        public string ConvertBytesToString(byte[] bytes)
        {
            return ValueConverter.ConvertBytesToString(bytes);
        }

        public byte[] ConvertStringToBytes(string value)
        {
            return ValueConverter.ConvertStringToBytes(value);
        }

    }
}
