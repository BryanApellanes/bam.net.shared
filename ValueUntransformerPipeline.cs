using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ValueUntransformerPipeline<TData> : ICollection<IValueUntransformer<byte[], byte[]>>, IValueUntransformer<string, TData>
    {
        List<IValueUntransformer<byte[], byte[]>> _decoders;
        public ValueUntransformerPipeline()
        {
            this._decoders = new List<IValueUntransformer<byte[], byte[]>>();
        }
        public IValueConverter BeforeDecodeConverter { get; set; }
        public IValueConverter<TData> AfterDecodeConverter { get; set; }
        public int Count => this._decoders.Count;

        public bool IsReadOnly => false;

        public IValueUntransformer<byte[], byte[]> this[int index]
        {
            get
            {
                return this._decoders[index];
            }
        }

        public void Add(IValueUntransformer<byte[], byte[]> item)
        {
            this._decoders.Add(item);
        }

        public void Clear()
        {
            this._decoders.Clear();
        }

        public bool Contains(IValueUntransformer<byte[], byte[]> item)
        {
            return this._decoders.Contains(item);
        }

        public void CopyTo(IValueUntransformer<byte[], byte[]>[] array, int arrayIndex)
        {
            this._decoders.CopyTo(array, arrayIndex);
        }

        public TData Untransform(string encoded)
        {
            byte[] converted = BeforeDecodeConverter.ConvertStringToBytes(encoded);
            byte[] data = converted;
            foreach(IValueUntransformer<byte[], byte[]> decoder in this._decoders)
            {
                data = decoder.Untransform(data);
            }
            TData result = AfterDecodeConverter.ConvertBytesToObject(data);
            return result;
        }

        public virtual IValueTransformer<TData, string> GetTransformer()
        {
            ValueTransformerPipeline<TData> encoder = new ValueTransformerPipeline<TData>();
            encoder.BeforeTransformConverter = AfterDecodeConverter;
            encoder.AfterTransformConverter = BeforeDecodeConverter;
            foreach(IValueUntransformer<byte[], byte[]> decoder in this._decoders)
            {
                encoder.Add(decoder.GetTransformer());
            }
            return encoder;
        }

        public IEnumerator<IValueUntransformer<byte[], byte[]>> GetEnumerator()
        {
            return this._decoders.GetEnumerator();
        }

        public bool Remove(IValueUntransformer<byte[], byte[]> item)
        {
            return this._decoders.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
