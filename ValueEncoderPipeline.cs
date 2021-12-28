using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ValueEncoderPipeline<TData> : ICollection<IValueTransformer<byte[], byte[]>>, IValueTransformer<TData, string>
    {
        List<IValueTransformer<byte[], byte[]>> _encoders;
        public ValueEncoderPipeline()
        {
            this._encoders = new List<IValueTransformer<byte[], byte[]>>();
        }

        public IValueConverter<TData> BeforeEncodeConverter { get; set; }
        public IValueConverter AfterEncodeConverter { get; set; }

        public bool IsReadOnly => false;

        public int Count => _encoders.Count;

        public IValueTransformer<byte[], byte[]> this[int index]
        {
            get
            {
                return this._encoders[index];
            }
        }

        public void Add(IValueTransformer<byte[], byte[]> item)
        {
            this._encoders.Add(item);
        }

        public void Clear()
        {
            this._encoders.Clear();
        }

        public bool Contains(IValueTransformer<byte[], byte[]> item)
        {
            return this._encoders.Contains(item);
        }

        public void CopyTo(IValueTransformer<byte[], byte[]>[] array, int arrayIndex)
        {
            this._encoders.CopyTo(array, arrayIndex);
        }

        public virtual string Transform(TData value)
        {
            byte[] converted = BeforeEncodeConverter.ConvertObjectToBytes(value);
            byte[] data = converted;

            foreach(IValueTransformer<byte[], byte[]> encoder in this._encoders)
            {
                data = encoder.Transform(data);
            }

            return AfterEncodeConverter.ConvertBytesToString(data);
        }

        public IValueUntransformer<string, TData> GetUntransformer()
        {
            ValueDecoderPipeline<TData> decoder = new ValueDecoderPipeline<TData>();
            decoder.BeforeDecodeConverter = this.AfterEncodeConverter;
            decoder.AfterDecodeConverter = this.BeforeEncodeConverter;
            foreach(IValueTransformer<byte[], byte[]> encoder in this._encoders)
            {
                decoder.Add(encoder.GetUntransformer());
            }
            return decoder;
        }

        public IEnumerator<IValueTransformer<byte[], byte[]>> GetEnumerator()
        {
            return this._encoders.GetEnumerator();
        }

        public bool Remove(IValueTransformer<byte[], byte[]> item)
        {
            return this._encoders.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
