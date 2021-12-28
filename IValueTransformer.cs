using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public interface IValueTransformer<TUntransformed, TTransformed>
    {
        IValueUntransformer<TTransformed, TUntransformed> GetUntransformer();

        /// <summary>
        /// When implemented in a derived class, encodes the specified value using
        /// the appropriate encoding implementation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        TTransformed Transform(TUntransformed value);
    }
}
