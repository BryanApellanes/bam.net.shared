using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonEncoder<TInput> : ValueEncoder<TInput, string>
    {
        public JsonEncoder() 
        {
            this.Encoding = Encoding.UTF8;
        }

        public Encoding Encoding { get; set; }

        public override TInput Decode(string output)
        {
            return GetDecoder().Decode(output);
        }

        public override string Encode(TInput value)
        {
            return value.ToJson();
        }

        public override IValueDecoder<string, TInput> GetDecoder()
        {
            return new JsonDecoder<TInput>();
        }
    }
}
