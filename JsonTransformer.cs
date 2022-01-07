using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class JsonTransformer<TInput> : ValueTransformer<TInput, string>
    {
        public JsonTransformer() 
        {
        }

        public override TInput Untransform(string output)
        {
            return GetUntransformer().ReverseTransform(output);
        }

        public override string Transform(TInput value)
        {
            return value.ToJson();
        }

        public override IValueReverseTransformer<string, TInput> GetUntransformer()
        {
            return new JsonReverseTransformer<TInput>();
        }
    }
}
