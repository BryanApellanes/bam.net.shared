using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class TypeMethodRequestRouter
    {
        RequestRouter _requestRouter;
        public TypeMethodRequestRouter(string pathName)
        {
            this._requestRouter = new RequestRouter(pathName);
        }

        public TypeMethodRoute GetTypeMethodRoute(string url)
        {
            RequestRoute requestRoute = _requestRouter.GetRequestRoute(url);
            if(requestRoute.IsValid)
            {
                return TypeMethodRoute.FromRequestRoute(requestRoute);
            }
            return null;
        }
    }
}
