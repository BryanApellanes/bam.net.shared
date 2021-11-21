/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.CoreServices;
using Bam.Net.Incubation;

namespace Bam.Net.ServiceProxy.Secure
{
    /// <summary>
    /// An ExecutionRequest that encrypts the result
    /// when executed.
    /// </summary>
    [Obsolete("use SecureServiceClient instead")]
    public class SecureExecutionRequest: ServiceProxyInvocation
    {
        public SecureExecutionRequest(IHttpContext context, string className, string methodName, string jsonArgs)
        {
            Args.ThrowIfNull(context, "context");
            Args.ThrowIfNullOrEmpty(className, "className");
            Args.ThrowIfNullOrEmpty(methodName, "methodName");

            this.ClassName = className;
            this.MethodName = methodName;
            this.ArgumentsAsJsonArrayOfJsonStrings = jsonArgs;
            //this.Ext = "json";
            this.Context = context;
            //this.IsUnencrypted = true;

            this.Executed += (o, t) =>
            {
                EncryptResult();
            };
        }

        protected virtual void EncryptResult()
        {
            string resultJson = Result.ToJson();
            string jsonCipher = Session.Encrypt(resultJson);
            Result = jsonCipher;
        }

        public static SecureExecutionRequest Create<T>(IHttpContext context, string methodName, params object[] parameters)
        {
            return Create<T>(context, methodName, Incubator.Default, parameters);
        }
        public static SecureExecutionRequest Create<T>(IHttpContext context, string methodName, ServiceRegistry serviceProvider, params object[] parameters)
        {
            string jsonParams = ApiArguments.ArgumentsToJsonArgumentsArray(parameters).ToJson();
            SecureExecutionRequest request = new SecureExecutionRequest(context, typeof(T).Name, methodName, jsonParams)
            {
                ServiceProvider = serviceProvider
            };
            return request;
        }

        protected internal override void Initialize()
        {
            // turn off initialization for this type
            //base.Initialize();
            IsInitialized = true;
        }

        protected internal override InvocationTargetInfo ResolveExecutionTargetInfo()
        {
            // effectively turns off parsing of the url since
            // everything is explicitly set already
            //base.ParseRequestUrl();
            return new InvocationTargetInfo
            {
                ClassName = ClassName,
                MethodName = MethodName,
                //Ext = Ext
            };
        }

        SecureSession _session;
        readonly object _sessionSync = new object();
        public SecureSession Session
        {
            get
            {
                return _sessionSync.DoubleCheckLock(ref _session, () => SecureSession.Get(Context));
            }
        }

        /// <summary>
        /// Decrypts the result and returns it as the specified type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetResultAs<T>()
        {
            string json = Session.Decrypt((string)Result);
            return json.FromJson<T>();
        }
    }
}
