using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonUntransformer<TUntransformed> : IValueUntransformer<string, TUntransformed>
    {
        public TUntransformed Untransform(string encoded)
        {
            return encoded.FromJson<TUntransformed>();
        }

        public IValueTransformer<TUntransformed, string> GetTransformer()
        {
            return (IValueTransformer<TUntransformed, string>)new JsonTransformer<string>();
        }
    }
}
