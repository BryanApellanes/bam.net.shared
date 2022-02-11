﻿using System;
using System.Text;

namespace Bam.Net
{
    public class ValueReverseTransformerPipeline<TData> : IValueReverseTransformer<byte[], TData>
    {
        public ValueReverseTransformerPipeline(ValueTranformerPipeline<TData> tranformerPipeline)
        {
            this.TranformerPipeline = tranformerPipeline;
        }

        public ValueTranformerPipeline<TData> TranformerPipeline { get; set; }

        public IValueTransformer<TData, byte[]> GetTransformer()
        {
            return this.TranformerPipeline;
        }

        public TData ReverseTransform(byte[] transformed)
        {
            IValueReverseTransformer<byte[], byte[]> reverseTransformer = TranformerPipeline.ByteTransformerPipeline.GetReverseTransformer();
            byte[] utf8 = reverseTransformer.ReverseTransform(transformed);
            string json = Encoding.UTF8.GetString(utf8);

            return json.FromJson<TData>();
        }
    }
}
