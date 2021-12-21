using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class CustomDecoder<TEncoded, TDecoded> : IValueDecoder<TEncoded, TDecoded>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public IHttpContext HttpContext { get; set; }

        public CustomEncoder<TDecoded, TEncoded> CustomEncoder { get; set; }

        public Func<IHttpContext, TEncoded, TDecoded> Decoder { get; set; }

        public object Clone()
        {
            object clone = new CustomDecoder<TEncoded, TDecoded>() { CustomEncoder = CustomEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            CustomDecoder<TEncoded, TDecoded> clone = new CustomDecoder<TEncoded, TDecoded>() { CustomEncoder = CustomEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public TDecoded Decode(TEncoded encoded)
        {
            return Decoder(HttpContext, encoded);
        }

        public IValueEncoder<TDecoded, TEncoded> GetEncoder()
        {
            return (IValueEncoder<TDecoded, TEncoded>)this.CustomEncoder;
        }
    }
}
