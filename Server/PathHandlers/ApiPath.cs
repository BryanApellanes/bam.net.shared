﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Server.PathHandlers
{
    public class ApiPath : NamedPath
    {
        public ApiPath()
        {
            PathName = "api";
        }

        public static ApiPath FromUri(Uri uri)
        {
            return NamedPath.FromUri<ApiPath>(uri);
        }
    }
}
