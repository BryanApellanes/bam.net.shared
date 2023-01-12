/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.CoreServices;
using Bam.Net.Data;
using Bam.Net.Encryption;
using Bam.Net.Incubation;
using Bam.Net.Logging;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;
using Bam.Net.Services;
using Bam.Net.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Bam.Net.Server.ServiceProxy
{
    public class ServiceProxyInvocation
    {
        public ServiceProxyInvocation()
        {
            OnAnyInstanciated(this);
        }

        public ServiceProxyInvocation(string className, string methodName, IHttpContext context = null) 
            : this(new WebServiceProxyDescriptors { WebServiceRegistry = Services.WebServiceRegistry.ForApplicationServiceRegistry(ApplicationServiceRegistry.ForProcess()) }, className, methodName, context)
        {
        }

        public ServiceProxyInvocation(WebServiceProxyDescriptors webServiceProxyDescriptors, string className, string methodName, IHttpContext context = null)
        {
            Args.ThrowIfNull(webServiceProxyDescriptors, nameof(webServiceProxyDescriptors));

            this.WebServiceProxyDescriptors = webServiceProxyDescriptors;
            this.Context = context ?? new HttpContextWrapper();
            this.ClassName = className;
            this.MethodName = methodName;

            OnAnyInstanciated(this);
        }

        public static ServiceProxyInvocation Create(WebServiceRegistry serviceRegistry, MethodInfo method, params ServiceProxyInvocationArgument[] arguments)
        {
            ServiceProxyInvocation request = new ServiceProxyInvocation()
            {
                WebServiceRegistry = serviceRegistry,
                MethodName = method.Name,
                MethodInfo = method,
                Arguments = arguments,
                ClassName = method.DeclaringType.Name,
                TargetType = method.DeclaringType
            };
            return request;
        }

        public virtual ServiceProxyInvocationValidationResult Validate()
        {
            ServiceProxyInvocationValidationResult validation = new ServiceProxyInvocationValidationResult(this);
            validation.Execute(Context);
            return validation;
        }

        ILogger _logger;
        public ILogger Logger
        {
            get => _logger ?? Log.Default;
            set => _logger = value;
        }

        public string ClassName
        {
            get;
            set;
        }

        public string MethodName
        {
            get;
            set;
        }

        public WebServiceProxyDescriptors WebServiceProxyDescriptors
        {
            get;
            set;
        }

        ApplicationServiceRegistry _applicationServiceRegistry;
        public ApplicationServiceRegistry ApplicationServiceRegistry
        {
            get
            {
                if (_applicationServiceRegistry == null)
                {
                    if (WebServiceProxyDescriptors != null)
                    {
                        _applicationServiceRegistry = WebServiceProxyDescriptors.ApplicationServiceRegistry;
                    }
                }

                return _applicationServiceRegistry;
            }
            set
            {
                _applicationServiceRegistry = value;
            }
        }

        WebServiceRegistry _webServiceRegistry;
        public WebServiceRegistry WebServiceRegistry
        {
            get
            {
                if(_webServiceRegistry == null)
                {
                    if (WebServiceProxyDescriptors != null)
                    {
                        _webServiceRegistry = WebServiceProxyDescriptors.WebServiceRegistry;
                    }
                }

                return _webServiceRegistry;
            }
            set
            {
                _webServiceRegistry = value;
            }
        }

        Type _targetType;
        public Type TargetType
        {
            get
            {
                if (_targetType == null && !string.IsNullOrWhiteSpace(ClassName))
                {
                    InvocationTarget = WebServiceRegistry.Get(ClassName, ApplicationServiceRegistry, out _targetType);
                }

                return _targetType;
            }
            set => _targetType = value;
        }

        object _invocationTarget;
        public object InvocationTarget
        {
            get
            {
                if (_invocationTarget == null)
                {
                    _invocationTarget = WebServiceRegistry.Get(ClassName, ApplicationServiceRegistry);
                }
                return _invocationTarget;
            }
            protected set => _invocationTarget = value;
        }

        MethodInfo _methodInfo;
        public MethodInfo MethodInfo
        {
            get
            {
                if (_methodInfo == null && TargetType != null)
                {
                    _methodInfo = TargetType.GetMethod(MethodName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                }
                return _methodInfo;
            }
            protected set => _methodInfo = value;
        }

        System.Reflection.ParameterInfo[] _parameterInfos;
        public System.Reflection.ParameterInfo[] ParameterInfos
        {
            get
            {
                if (_parameterInfos == null && MethodInfo != null)
                {
                    List<System.Reflection.ParameterInfo> parameters = MethodInfo.GetParameters().ToList();
                    parameters.Sort((x, y) => x.Position.CompareTo(y.Position));
                    _parameterInfos = parameters.ToArray();
                }

                return _parameterInfos;
            }
        }

        public ServiceProxyInvocationArgument[] Arguments
        {
            get;
            set;
        }

        protected virtual object[] GetArguments()
        {
            // TODO: consider extracting this functionality into an ExecutionResponse class that takes the request and resolves the 
            // relevant bits using an IExecutionRequestTargetResolver stated in ResolveExecutionTargetInfo.
            //           AND/OR
            // TODO: consider breaking this class up into specific ExecutionRequest implementations that encapsulate the style of input parameters/arguments
            //  JsonParamsExecutionRequest, OrderedHttpArgsExecutionRequest, FormEncodedPostExecutionRequest, QueryStringParametersExecutionRequest.
            //  The type of the request should be resolved by examining the ContentType

            // see ExecutionRequestResolver.ResolveExecutionRequest

            // This method is becoming a little bloated
            // due to accommodating too many input paths.
            // This will need to be refactored IF
            // changes continue to be necessary
            // 07/29/2018 - +1 added notes -BA
            // see commit 2526558ea460852c033d1151dc190308a9feaefd
            string jsonParams;
            object[] result = new object[] { };
            /*object[] result = new object[] { }; ;
            if (HttpArgs.Has("jsonParams", out string jsonParams))
            {
                string[] jsonStrings = jsonParams.FromJson<string[]>();
                result = GetJsonArguments(jsonStrings);
            }
            else */
            if (!string.IsNullOrEmpty(string.Empty))//ArgumentsAsJsonArrayOfJsonStrings))
            {
                // POST: bam.invoke
                string[] jsonStrings = new string[] { };//ArgumentsAsJsonArrayOfJsonStrings.FromJson<string[]>();

                result = new object[] { };//GetJsonArguments(jsonStrings);
            }
            else if (Request != null)// && InputString.Length > 0)
            {
                // POST: probably from a form
               // Queue<string> inputValues = new Queue<string>(InputString.Split('&'));

                result = GetFormArguments(null);
            }
            else if (Request != null)
            {
                // GET: parse the querystring
                //ViewName = Request.QueryString["view"];
                if (string.IsNullOrEmpty(""))//ViewName))
                {
                    //ViewName = "Default";
                }

                jsonParams = Request.QueryString["jsonParams"];
                bool numbered = !string.IsNullOrEmpty(Request.QueryString["numbered"]) ? true : false;
                bool named = !numbered;

                if (!string.IsNullOrEmpty(jsonParams))
                {
                    dynamic o = JsonConvert.DeserializeObject<dynamic>(jsonParams);
                    string[] jsonStrings = ((string)o["jsonParams"]).FromJson<string[]>();
                    result = new object[] { };//GetJsonArguments(jsonStrings);
                }
                /*                else if (named)
                                {
                                    result = GetNamedQueryStringArguments();
                                }
                                else
                                {
                                    result = GetNumberedQueryStringArguments();
                                }*/
            }

            return result;
        }
        // TOOD: encapsulate this as a ServiceProxyInvocationFormArguments
        // parse form input
        private object[] GetFormArguments(Queue<string> inputValues)
        {
            object[] result = new object[ParameterInfos.Length]; // holder for results

            for (int i = 0; i < ParameterInfos.Length; i++)
            {
                System.Reflection.ParameterInfo param = ParameterInfos[i];
                Type currentParameterType = param.ParameterType;
                object parameterValue = GetParameterValue(inputValues, currentParameterType);

                result[i] = parameterValue;
            }
            return result;
        }

        private static object GetParameterValue(Queue<string> inputValues, Type currentParameterType)
        {
            return GetParameterValue(inputValues, currentParameterType, 0);
        }

        // this implementation accounts for a complex object having properties of types that potentially have properties named the same
        // as the parent type
        // {Name: "man", Son: {Name: "boy"}}
        // comma delimits Name as man,boy
        private static object GetParameterValue(Queue<string> inputValues, Type currentParameterType, int recursionThusFar)
        {
            object parameterValue = currentParameterType.Construct();

            List<PropertyInfo> properties = new List<PropertyInfo>(currentParameterType.GetProperties());
            properties.Sort((l, r) => l.MetadataToken.CompareTo(r.MetadataToken));

            foreach (PropertyInfo propertyOfCurrentType in properties)
            {
                if (!propertyOfCurrentType.HasCustomAttributeOfType<ExcludeAttribute>())
                {
                    Type typeOfCurrentProperty = propertyOfCurrentType.PropertyType;
                    // string 
                    // int 
                    // long
                    // decimal
                    if (typeOfCurrentProperty == typeof(string) ||
                        typeOfCurrentProperty == typeof(int) ||
                        typeOfCurrentProperty == typeof(long) ||
                        typeOfCurrentProperty == typeof(decimal))
                    {
                        string input = inputValues.Dequeue();
                        string[] keyValue = input.Split('=');
                        string key = null;
                        object value = null;
                        if (keyValue.Length > 0)
                        {
                            key = keyValue[0];
                        }

                        if (keyValue.Length == 1)
                        {
                            value = Convert.ChangeType(string.Empty, typeOfCurrentProperty);
                        }
                        else if (keyValue.Length == 2)
                        {
                            // 4.0 implementation 
                            value = Convert.ChangeType(Uri.UnescapeDataString(keyValue[1]), typeOfCurrentProperty);

                            // 4.5 implementation
                            //value = Convert.ChangeType(WebUtility.UrlDecode(keyValue[1]), typeOfCurrentProperty);
                        }

                        if (propertyOfCurrentType.Name.Equals(key))
                        {
                            propertyOfCurrentType.SetValue(parameterValue, value, null);
                        }
                        else
                        {
                            throw Args.Exception("Unexpected key value {0}, expected {1}", key, propertyOfCurrentType.Name);
                        }
                    }
                    else
                    {
                        //if (recursionThusFar <= MaxRecursion)
                        {
                            // object
                            propertyOfCurrentType.SetValue(parameterValue, GetParameterValue(inputValues, propertyOfCurrentType.PropertyType, ++recursionThusFar), null);
                        }
                    }
                }
            }
            return parameterValue;
        }

        protected internal IHttpContext Context
        {
            get;
            set;
        }

        protected internal IRequest Request
        {
            get => Context?.Request;
            set => Context.Request = value;
        }

        protected internal IResponse Response
        {
            get => Context?.Response;
            set => Context.Response = value;
        }


        /// <summary>
        /// The result of executing the request
        /// </summary>
        public object Result
        {
            get;
            internal set;
        }

        public Exception Exception
        {
            get;
            internal set;
        }

        public static event Action<ServiceProxyInvocation> AnyInstanciated;
        protected static void OnAnyInstanciated(ServiceProxyInvocation request)
        {
            AnyInstanciated?.Invoke(request);
        }

        public static event Action<ServiceProxyInvocation, object> AnyExecuting;
        protected void OnAnyExecuting(object target)
        {
            AnyExecuting?.Invoke(this, target);
        }
        public static event Action<ServiceProxyInvocation, object> AnyExecuted;
        protected void OnAnyExecuted(object target)
        {
            AnyExecuted?.Invoke(this, target);
        }

        public event Action<ServiceProxyInvocation, object> Executing;
        protected void OnExecuting(object target)
        {
            Executing?.Invoke(this, target);
        }

        public event Action<ServiceProxyInvocation, object> AnyExecutionException;

        protected void OnAnyExecutionException(object target)
        {
            AnyExecutionException?.Invoke(this, target);
        }

        public event Action<ServiceProxyInvocation, object> ExecutionException;

        protected void OnExecutionException(object target)
        {
            ExecutionException?.Invoke(this, target);
        }

        public event Action<ServiceProxyInvocation, object> Executed;
        protected void OnExecuted(object target)
        {
            Executed?.Invoke(this, target);
        }

        public event Action<ServiceProxyInvocation, object> ContextSet;
        protected void OnContextSet(object target)
        {
            ContextSet?.Invoke(this, target);
        }

        public event Action<ServiceProxyInvocation, object> WebServiceRegistrySet;
        protected void OnWebServiceRegistrySet(object target)
        {
            WebServiceRegistrySet?.Invoke(this, target);
        }

        public event Action<ServiceProxyInvocation, object> ApplicationServiceRegistrySet;
        protected void OnApplicationServiceRegistrySet(object target)
        {
            ApplicationServiceRegistrySet?.Invoke(this, target);
        }

        public bool Execute()
        {
            return Execute(InvocationTarget, true);
        }

        public bool Execute<T>(out T result)
        {
            bool wasSuccessful = Execute(out object innerResult);
            result = (T)innerResult;
            return wasSuccessful;
        }

        public bool Execute(out object result)
        {
            bool success = Execute(InvocationTarget, true);
            result = Result;
            return success;
        }

        public bool Execute(object target, bool validate = true)
        {
            bool success = false;
            if (validate)
            {
                ServiceProxyInvocationValidationResult validation = Validate();
                if (!validation.Success)
                {
                    Result = validation;
                }
            }

            if (Result == null)
            {
                try
                {
                    target = SetContext(target);
                    target = SetApplicationServiceRegistry(target);
                    target = SetWebServiceRegistry(target);
                    OnAnyExecuting(target);
                    OnExecuting(target);
                    Result = MethodInfo.Invoke(target, Arguments.Select(arg => arg.Value).ToArray());
                    OnExecuted(target);
                    OnAnyExecuted(target);
                    success = true;
                }
                catch (Exception ex)
                {
                    Result = $"{ex.GetInnerException()?.Message} \r\n\r\n\t{ex.GetInnerException()?.StackTrace}";
                    Exception = ex;
                    success = false;
                    OnExecutionException(target);
                    OnAnyExecutionException(target);
                }
            }

            return success;
        }

        protected internal object SetContext(object target)
        {
            object result = target;
            if (target is IRequiresHttpContext takesContext)
            {
                takesContext = (IRequiresHttpContext)takesContext.Clone();
                takesContext.HttpContext = Context;
                OnContextSet(takesContext);
                result = takesContext;
            }
            return result;
        }

        protected internal object SetApplicationServiceRegistry(object target)
        {
            object result = target;
            if (target is IHasApplicationServiceRegistry hasServiceRegistry)
            {
                hasServiceRegistry.ApplicationServiceRegistry = ApplicationServiceRegistry;
                OnApplicationServiceRegistrySet(target);
                ApplicationServiceRegistry.SetInjectionProperties(target);
                result = hasServiceRegistry;
            }
            return result;
        }

        protected internal object SetWebServiceRegistry(object target)
        {
            object result = target;
            if (target is IHasWebServiceRegistry hasServiceRegistry)
            {
                hasServiceRegistry.WebServiceRegistry = WebServiceRegistry;
                OnWebServiceRegistrySet(target);
                result = hasServiceRegistry;
            }
            return result;
        }        
    }
}
