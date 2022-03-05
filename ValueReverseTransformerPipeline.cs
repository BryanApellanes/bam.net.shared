using System;
using System.Text;

namespace Bam.Net
{
    public class ValueReverseTransformerPipeline<TData> : IValueReverseTransformer<byte[], TData>
    {
        public ValueReverseTransformerPipeline(ValueTransformerPipeline<TData> tranformerPipeline)
        {
            this.TranformerPipeline = tranformerPipeline;
        }

        public ValueTransformerPipeline<TData> TranformerPipeline { get; set; }

        public IValueTransformer<TData, byte[]> GetTransformer()
        {
            return this.TranformerPipeline;
        }

        public TData ReverseTransform(byte[] transformed)
        {
            IValueReverseTransformer<byte[], byte[]> reverseTransformer = TranformerPipeline.ByteTransformerPipeline.GetReverseTransformer();
            byte[] utf8 = reverseTransformer.ReverseTransform(transformed);
            string reversedString = Encoding.UTF8.GetString(utf8);

            return ConvertStringToData(reversedString);
        }

        public TData ConvertStringToData(string stringValue)
        {
            return stringValue.FromJson<TData>();
        }
    }
}
