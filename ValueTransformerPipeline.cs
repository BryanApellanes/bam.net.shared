using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class ValueTransformerPipeline<TData> : ICollection<IValueTransformer<byte[], byte[]>>, IValueTransformer<TData, string>
    {
        readonly List<IValueTransformer<byte[], byte[]>> _transformers;
        public ValueTransformerPipeline()
        {
            this._transformers = new List<IValueTransformer<byte[], byte[]>>();
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

        public int Count => _transformers.Count;

        public IValueTransformer<byte[], byte[]> this[int index]
        {
            get
            {
                return this._transformers[index];
            }
        }

        public void Add(IValueTransformer<byte[], byte[]> item)
        {
            this._transformers.Add(item);
        }

        public void Clear()
        {
            this._transformers.Clear();
        }

        public bool Contains(IValueTransformer<byte[], byte[]> item)
        {
            return this._transformers.Contains(item);
        }

        public void CopyTo(IValueTransformer<byte[], byte[]>[] array, int arrayIndex)
        {
            this._transformers.CopyTo(array, arrayIndex);
        }

        public virtual string Transform(TData value)
        {
            byte[] converted = BeforeTransformConverter.ConvertObjectToBytes(value);
            byte[] data = converted;

            foreach(IValueTransformer<byte[], byte[]> transformer in this._transformers)
            {
                data = transformer.Transform(data);
            }

            return AfterTransformConverter.ConvertBytesToString(data);
        }

        public IValueReverseTransformer<string, TData> GetReverseTransformer()
        {
            ValueReverseTransformerPipeline<TData> untransformer = new ValueReverseTransformerPipeline<TData>();
            untransformer.BeforeDecodeConverter = this.AfterTransformConverter;
            untransformer.AfterDecodeConverter = this.BeforeTransformConverter;
            this._transformers.BackwardsEach(transformer => untransformer.Add(transformer.GetReverseTransformer()));
            return untransformer;
        }

        public IEnumerator<IValueTransformer<byte[], byte[]>> GetEnumerator()
        {
            return this._transformers.GetEnumerator();
        }

        public bool Remove(IValueTransformer<byte[], byte[]> item)
        {
            return this._transformers.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
