using Bam.Net.Server.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class DefaultApiArgumentEncoder : ApiArgumentEncoder
    {
        
        static DefaultApiArgumentEncoder _current;
        static object _currentLock = new object();
        public static DefaultApiArgumentEncoder Current
        {
            get
            {
                return _currentLock.DoubleCheckLock(ref _current, () => new DefaultApiArgumentEncoder());
            }
        }
    }
}
