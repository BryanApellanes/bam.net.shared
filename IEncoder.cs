using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public interface IEncoder : IDecoder
    {
        /// <summary>
        /// When implemented in a derived class, encodes the specified value using
        /// the appropriate encoding implementation and returns the base64 encoded result.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        byte[] Encode(object value);
    }
}
