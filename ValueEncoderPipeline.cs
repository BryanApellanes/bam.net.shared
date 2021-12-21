using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ValueEncoderPipeline<TData> : ICollection<IValueEncoder<byte[], byte[]>>, IValueEncoder<TData, string>
    {
        List<IValueEncoder<byte[], byte[]>> _encoders;
        public ValueEncoderPipeline()
        {
            this._encoders = new List<IValueEncoder<byte[], byte[]>>();
        }

        public IValueConverter<TData> BeforeEncodeConverter { get; set; }
        public IValueConverter AfterEncodeConverter { get; set; }

        public bool IsReadOnly => false;

        public int Count => _encoders.Count;

        public IValueEncoder<byte[], byte[]> this[int index]
        {
            get
            {
                return this._encoders[index];
            }
        }

        public void Add(IValueEncoder<byte[], byte[]> item)
        {
            this._encoders.Add(item);
        }

        public void Clear()
        {
            this._encoders.Clear();
        }

        public bool Contains(IValueEncoder<byte[], byte[]> item)
        {
            return this._encoders.Contains(item);
        }

        public void CopyTo(IValueEncoder<byte[], byte[]>[] array, int arrayIndex)
        {
            this._encoders.CopyTo(array, arrayIndex);
        }

        public string Encode(TData value)
        {
            byte[] converted = BeforeEncodeConverter.ConvertObjectToBytes(value);
            byte[] data = converted;

            foreach(IValueEncoder<byte[], byte[]> encoder in this._encoders)
            {
                data = encoder.Encode(data);
            }

            return AfterEncodeConverter.ConvertBytesToString(data);
        }

        public IValueDecoder<string, TData> GetDecoder()
        {
            ValueDecoderPipeline<TData> decoder = new ValueDecoderPipeline<TData>();
            decoder.BeforeDecodeConverter = this.AfterEncodeConverter;
            decoder.AfterDecodeConverter = this.BeforeEncodeConverter;
            foreach(IValueEncoder<byte[], byte[]> encoder in this._encoders)
            {
                decoder.Add(encoder.GetDecoder());
            }
            return decoder;
        }

        public IEnumerator<IValueEncoder<byte[], byte[]>> GetEnumerator()
        {
            return this._encoders.GetEnumerator();
        }

        public bool Remove(IValueEncoder<byte[], byte[]> item)
        {
            return this._encoders.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
