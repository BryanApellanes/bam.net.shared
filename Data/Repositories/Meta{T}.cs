using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Data.Repositories
{
    /// <summary>
    /// Provides meta data about persisted or persistable objects. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Bam.Net.Data.Repositories.Meta" />
    [Serializable]
    public class Meta<T> : Meta
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Meta{T}"/> class.
        /// </summary>
        public Meta() : base()
        {
            Type = typeof(T);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Meta{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="objectReaderWriter">The object reader writer.</param>
        public Meta(T data, IObjectPersister objectReaderWriter) : base(data, objectReaderWriter)
        {
            Type = typeof(T);
        }

        /// <summary>
        /// Gets or sets the typed data.
        /// </summary>
        /// <value>
        /// The typed data.
        /// </value>
        public T TypedData
        {
            get
            {
                return (T)Data;
            }
            set
            {
                Data = value;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Meta{T}"/> to T.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator T(Meta<T> meta)
        {
            return meta.TypedData;
        }
    }

}
