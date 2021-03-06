/*
	Copyright © Bryan Apellanes 2015  
*/
using System.Collections.Generic;
using System.Reflection;
using Bam.Net.ServiceProxy;

namespace Bam.Net.CoreServices
{
    public partial class ProxyMethodModel
    {
        public ProxyMethodModel(MethodInfo method, params Assembly[] assembliesToreference)
            : this(new MethodGenerationInfo(method), assembliesToreference)
        { }

        public ProxyMethodModel(MethodGenerationInfo methodGenInfo, params Assembly[] assembliesToReference)
        {
            this.MethodGenerationInfo = methodGenInfo;
            List<Assembly> assemblies = new List<Assembly>(assembliesToReference);
            assemblies.AddRange(methodGenInfo.ReferenceAssemblies);
            this.ReferenceAssemblies = assemblies.ToArray();
        }

        public MethodGenerationInfo MethodGenerationInfo { get; private set; }

        public string ReturnType
        {
            get
            {
                return MethodGenerationInfo.ReturnTypeCodeString;
            }
        }

        public string MethodName
        {
            get
            {
                return MethodGenerationInfo.Method.Name;
            }
        }
        public string Signature
        {
            get
            {
                return MethodGenerationInfo.MethodSignature;
            }
        }

        public string ParameterInstances
        {
            get
            {
                return MethodGenerationInfo.ParameterInstances;
            }
        }
        public string OverrideOrNew
        {
            get
            {
                return MethodGenerationInfo.Method.IsVirtual ? "override" : "new";
            }
        }

        public string Invocation
        {
            get
            {
                bool isVoidReturn = MethodGenerationInfo.IsVoidReturn;
                string returnOrBlank = MethodGenerationInfo.IsVoidReturn ? "" : "return ";                
                return string.Format("{0}_proxyClient.{1}", returnOrBlank, MethodGenerationInfo.Method.Name);
            }
        }

        public Assembly[] ReferenceAssemblies
        {
            get;
            set;
        }


    }
}
