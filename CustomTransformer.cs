using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class CustomTransformer<TDecoded, TEncoded> : ValueTransformer<TDecoded, TEncoded>
    {
        public CustomTransformer()
        {
            this.CustomUntransformer = new CustomReverseTransformer<TEncoded, TDecoded>() { CustomTransformer = this };
        }

        public CustomTransformer(Func<TDecoded, TEncoded> encoder, Func<IHttpContext, TEncoded, TDecoded> decoder)
        {
            this.Encoder = encoder;
            this.CustomUntransformer = new CustomReverseTransformer<TEncoded, TDecoded>() { CustomTransformer = this, Untransformer = decoder };
        }

        public CustomReverseTransformer<TEncoded, TDecoded> CustomUntransformer { get; internal set; }

        public Func<TDecoded, TEncoded> Encoder { get; set; }

        public override TDecoded Untransform(TEncoded output)
        {
            return GetUntransformer().ReverseTransform(output);
        }

        public override TEncoded Transform(TDecoded input)
        {
            return Encoder(input);
        }

        public override IValueReverseTransformer<TEncoded, TDecoded> GetUntransformer()
        {
            return (IValueReverseTransformer<TEncoded, TDecoded>)this.CustomUntransformer;
        }
    }
}
