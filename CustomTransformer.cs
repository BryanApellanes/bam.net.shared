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
            this.CustomUntransformer = new CustomUntransformer<TEncoded, TDecoded>() { CustomTransformer = this };
        }

        public CustomTransformer(Func<TDecoded, TEncoded> encoder, Func<IHttpContext, TEncoded, TDecoded> decoder)
        {
            this.Encoder = encoder;
            this.CustomUntransformer = new CustomUntransformer<TEncoded, TDecoded>() { CustomTransformer = this, Untransformer = decoder };
        }

        public CustomUntransformer<TEncoded, TDecoded> CustomUntransformer { get; internal set; }

        public Func<TDecoded, TEncoded> Encoder { get; set; }

        public override TDecoded Untransform(TEncoded output)
        {
            return GetUntransformer().Untransform(output);
        }

        public override TEncoded Transform(TDecoded input)
        {
            return Encoder(input);
        }

        public override IValueUntransformer<TEncoded, TDecoded> GetUntransformer()
        {
            return (IValueUntransformer<TEncoded, TDecoded>)this.CustomUntransformer;
        }
    }
}
