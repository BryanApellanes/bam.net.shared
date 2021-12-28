using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class CustomUntransformer<TEncoded, TDecoded> : IValueUntransformer<TEncoded, TDecoded>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public IHttpContext HttpContext { get; set; }

        public CustomTransformer<TDecoded, TEncoded> CustomEncoder { get; set; }

        public Func<IHttpContext, TEncoded, TDecoded> Untransformer { get; set; }

        public object Clone()
        {
            object clone = new CustomUntransformer<TEncoded, TDecoded>() { CustomEncoder = CustomEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            CustomUntransformer<TEncoded, TDecoded> clone = new CustomUntransformer<TEncoded, TDecoded>() { CustomEncoder = CustomEncoder };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public TDecoded Untransform(TEncoded encoded)
        {
            return Untransformer(HttpContext, encoded);
        }

        public IValueTransformer<TDecoded, TEncoded> GetTransformer()
        {
            return (IValueTransformer<TDecoded, TEncoded>)this.CustomEncoder;
        }
    }
}
