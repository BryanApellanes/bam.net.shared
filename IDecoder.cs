﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public interface IDecoder
    {
        /// <summary>
        /// When implemented in a derived class, base64 decodes the specified input then
        /// decodes the result using the appropriate decoding implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="base64EncodedString"></param>
        /// <returns></returns>
        T Decode<T>(byte[] encoded);
    }
}
