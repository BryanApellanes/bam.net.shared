using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ValueTransformerPipeline<TData> : ICollection<IValueTransformer<byte[], byte[]>>, IValueTransformer<TData, string>
    {
        readonly List<IValueTransformer<byte[], byte[]>> _encoders;
        public ValueTransformerPipeline()
        {
            this._encoders = new List<IValueTransformer<byte[], byte[]>>();
        }

        /// <summary>
        /// Gets or sets the converter used to convert the input data into a byte array.
        /// </summary>
        public IValueConverter<TData> BeforeTransformConverter { get; set; }

        /// <summary>
        /// Gets or sets the converter used to convert the final byte array into a string.
        /// </summary>
        public IValueConverter AfterTransformConverter { get; set; }

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
            byte[] converted = BeforeTransformConverter.ConvertObjectToBytes(value);
            byte[] data = converted;

            foreach(IValueTransformer<byte[], byte[]> encoder in this._encoders)
            {
                data = encoder.Transform(data);
            }

            return AfterTransformConverter.ConvertBytesToString(data);
        }

        public IValueUntransformer<string, TData> GetUntransformer()
        {
            ValueUntransformerPipeline<TData> decoder = new ValueUntransformerPipeline<TData>();
            decoder.BeforeDecodeConverter = this.AfterTransformConverter;
            decoder.AfterDecodeConverter = this.BeforeTransformConverter;
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
