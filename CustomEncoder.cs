using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class CustomEncoder<TDecoded, TEncoded> : ValueEncoder<TDecoded, TEncoded>
    {
        public CustomEncoder()
        {
            this.CustomDecoder = new CustomDecoder<TEncoded, TDecoded>() { CustomEncoder = this };
        }

        public CustomEncoder(Func<TDecoded, TEncoded> encoder, Func<IHttpContext, TEncoded, TDecoded> decoder)
        {
            this.Encoder = encoder;
            this.CustomDecoder = new CustomDecoder<TEncoded, TDecoded>() { CustomEncoder = this, Decoder = decoder };
        }

        public CustomDecoder<TEncoded, TDecoded> CustomDecoder { get; internal set; }

        public Func<TDecoded, TEncoded> Encoder { get; set; }

        public override TDecoded Decode(TEncoded output)
        {
            return GetDecoder().Decode(output);
        }

        public override TEncoded Encode(TDecoded input)
        {
            return Encoder(input);
        }

        public override IValueDecoder<TEncoded, TDecoded> GetDecoder()
        {
            return (IValueDecoder<TEncoded, TDecoded>)this.CustomDecoder;
        }
    }
}
