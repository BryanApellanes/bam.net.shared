/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bam.Net.ServiceProxy;

namespace Bam.Net.Server
{
    public class ResponderList: Responder
    {
        readonly List<IHttpResponder> _responders;
        public ResponderList(BamConf conf, IEnumerable<IHttpResponder> responders)
            : base(conf)
        {
            this._responders = new List<IHttpResponder>(responders);
        }

        public void AddResponders(params IHttpResponder[] responder)
        {
            _responders.AddRange(responder);
        }

        public override bool MayRespond(IHttpContext context)
        {
            return true;
        }

        /// <summary>
        /// The responder that handled the request if any
        /// </summary>
        public IHttpResponder HandlingResponder { get; set; }

        #region IResponder Members

        public override bool TryRespond(IHttpContext context)
        {
            bool handled = false;
            foreach (IHttpResponder r in _responders)
            {
                if (r.Respond(context))
                {
                    HandlingResponder = r;
                    handled = true;
                    break;
                }
            }

            return handled;
        }

        #endregion
    }
}
