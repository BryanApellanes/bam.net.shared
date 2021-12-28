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
            return GetUntransformer().Untransform(output);
        }

        public override string Transform(TInput value)
        {
            return value.ToJson();
        }

        public override IValueUntransformer<string, TInput> GetUntransformer()
        {
            return new JsonUntransformer<TInput>();
        }
    }
}
