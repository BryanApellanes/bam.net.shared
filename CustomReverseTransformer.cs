using Bam.Net.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
    public class CustomReverseTransformer<TEncoded, TDecoded> : IValueReverseTransformer<TEncoded, TDecoded>, IRequiresHttpContext, ICloneable, IContextCloneable
    {
        public IHttpContext HttpContext { get; set; }

        public CustomTransformer<TDecoded, TEncoded> CustomTransformer { get; set; }

        public Func<IHttpContext, TEncoded, TDecoded> Untransformer { get; set; }

        public object Clone()
        {
            object clone = new CustomReverseTransformer<TEncoded, TDecoded>() { CustomTransformer = CustomTransformer };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            return clone;
        }

        public object Clone(IHttpContext context)
        {
            CustomReverseTransformer<TEncoded, TDecoded> clone = new CustomReverseTransformer<TEncoded, TDecoded>() { CustomTransformer = CustomTransformer };
            clone.CopyProperties(this);
            clone.CopyEventHandlers(this);
            clone.HttpContext = context;
            return clone;
        }

        public object CloneInContext()
        {
            return Clone(HttpContext);
        }

        public TDecoded ReverseTransform(TEncoded encoded)
        {
            return Untransformer(HttpContext, encoded);
        }

        public IValueTransformer<TDecoded, TEncoded> GetTransformer()
        {
            return (IValueTransformer<TDecoded, TEncoded>)this.CustomTransformer;
        }
    }
}
