using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Server
{
    public class TypeMethodRoute
    {
        public const string Route = "{ClassName}/{MethodName}?{QueryParameters}";

        public string ClassName{ get; set; }
        public string MethodName { get; set; }
        public string QueryParameters { get; set; }

        public static TypeMethodRoute FromRequestRoute(RequestRoute requestRoute)
        {
            RouteParser parser = new RouteParser(Route);
            Dictionary<string, string> values = parser.ParseRouteInstance(requestRoute.PathAndQuery);
            return values.ToInstance<TypeMethodRoute>();
        }
    }
}
