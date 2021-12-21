using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ValueDecoderPipeline<TData> : ICollection<IValueDecoder<byte[], byte[]>>, IValueDecoder<string, TData>
    {
        List<IValueDecoder<byte[], byte[]>> _decoders;
        public ValueDecoderPipeline()
        {
            this._decoders = new List<IValueDecoder<byte[], byte[]>>();
        }
        public IValueConverter BeforeDecodeConverter { get; set; }
        public IValueConverter<TData> AfterDecodeConverter { get; set; }
        public int Count => this._decoders.Count;

        public bool IsReadOnly => false;

        public IValueDecoder<byte[], byte[]> this[int index]
        {
            get
            {
                return this._decoders[index];
            }
        }

        public void Add(IValueDecoder<byte[], byte[]> item)
        {
            this._decoders.Add(item);
        }

        public void Clear()
        {
            this._decoders.Clear();
        }

        public bool Contains(IValueDecoder<byte[], byte[]> item)
        {
            return this._decoders.Contains(item);
        }

        public void CopyTo(IValueDecoder<byte[], byte[]>[] array, int arrayIndex)
        {
            this._decoders.CopyTo(array, arrayIndex);
        }

        public TData Decode(string encoded)
        {
            byte[] converted = BeforeDecodeConverter.ConvertStringToBytes(encoded);
            byte[] data = converted;
            foreach(IValueDecoder<byte[], byte[]> decoder in this._decoders)
            {
                data = decoder.Decode(data);
            }
            TData result = AfterDecodeConverter.ConvertBytesToObject(data);
            return result;
        }

        public IValueEncoder<TData, string> GetEncoder()
        {
            ValueEncoderPipeline<TData> encoder = new ValueEncoderPipeline<TData>();
            encoder.BeforeEncodeConverter = AfterDecodeConverter;
            encoder.AfterEncodeConverter = BeforeDecodeConverter;
            foreach(IValueDecoder<byte[], byte[]> decoder in this._decoders)
            {
                encoder.Add(decoder.GetEncoder());
            }
            return encoder;
        }

        public IEnumerator<IValueDecoder<byte[], byte[]>> GetEnumerator()
        {
            return this._decoders.GetEnumerator();
        }

        public bool Remove(IValueDecoder<byte[], byte[]> item)
        {
            return this._decoders.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
