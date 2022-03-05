/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.ServiceProxy.Encryption
{
    /// <summary>
    /// Attribute used to adorn classes or methods that require
    /// authentication or authorization.  Implicitly requires
    /// application level encryption.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiHmacKeyRequiredAttribute: EncryptAttribute
    {
    }
}