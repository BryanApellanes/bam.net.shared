using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public interface IValueEncoder<TDecoded, TEncoded>
    {
        IValueDecoder<TEncoded, TDecoded> GetDecoder();

        /// <summary>
        /// When implemented in a derived class, encodes the specified value using
        /// the appropriate encoding implementation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        TEncoded Encode(TDecoded value);
    }
}
