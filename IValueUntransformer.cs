using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public interface IValueUntransformer<TTransformed, TUntransformed>
    {
        IValueTransformer<TUntransformed, TTransformed> GetTransformer();

        TUntransformed Untransform(TTransformed transformed);
    }
}
