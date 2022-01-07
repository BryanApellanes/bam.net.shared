using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ValueReverseTransformerPipeline<TData> : ICollection<IValueReverseTransformer<byte[], byte[]>>, IValueReverseTransformer<string, TData>
    {
        List<IValueReverseTransformer<byte[], byte[]>> _reverseTransformers;
        public ValueReverseTransformerPipeline()
        {
            this._reverseTransformers = new List<IValueReverseTransformer<byte[], byte[]>>();
        }
        public IValueConverter BeforeDecodeConverter { get; set; }
        public IValueConverter<TData> AfterDecodeConverter { get; set; }
        public int Count => this._reverseTransformers.Count;

        public bool IsReadOnly => false;

        public IValueReverseTransformer<byte[], byte[]> this[int index]
        {
            get
            {
                return this._reverseTransformers[index];
            }
        }

        public void Add(IValueReverseTransformer<byte[], byte[]> item)
        {
            this._reverseTransformers.Add(item);
        }

        public void Clear()
        {
            this._reverseTransformers.Clear();
        }

        public bool Contains(IValueReverseTransformer<byte[], byte[]> item)
        {
            return this._reverseTransformers.Contains(item);
        }

        public void CopyTo(IValueReverseTransformer<byte[], byte[]>[] array, int arrayIndex)
        {
            this._reverseTransformers.CopyTo(array, arrayIndex);
        }

        public TData ReverseTransform(string encoded)
        {
            byte[] converted = BeforeDecodeConverter.ConvertStringToBytes(encoded);
            byte[] data = converted;
            foreach(IValueReverseTransformer<byte[], byte[]> decoder in this._reverseTransformers)
            {
                data = decoder.ReverseTransform(data);
            }
            TData result = AfterDecodeConverter.ConvertBytesToObject(data);
            return result;
        }

        public virtual IValueTransformer<TData, string> GetTransformer()
        {
            ValueTransformerPipeline<TData> encoder = new ValueTransformerPipeline<TData>();
            encoder.BeforeTransformConverter = AfterDecodeConverter;
            encoder.AfterTransformConverter = BeforeDecodeConverter;
            foreach(IValueReverseTransformer<byte[], byte[]> decoder in this._reverseTransformers)
            {
                encoder.Add(decoder.GetTransformer());
            }
            return encoder;
        }

        public IEnumerator<IValueReverseTransformer<byte[], byte[]>> GetEnumerator()
        {
            return this._reverseTransformers.GetEnumerator();
        }

        public bool Remove(IValueReverseTransformer<byte[], byte[]> item)
        {
            return this._reverseTransformers.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
