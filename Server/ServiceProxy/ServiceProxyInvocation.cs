/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.CoreServices;
using Bam.Net.Data;
using Bam.Net.Encryption;
using Bam.Net.Incubation;
using Bam.Net.Logging;
using Bam.Net.ServiceProxy;
using Bam.Net.ServiceProxy.Secure;
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
        public const int DefaultMaxRecursion = 5;

        public ServiceProxyInvocation()
        {
            OnAnyInstanciated(this);
        }

        public ServiceProxyInvocation(string className, string methodName, IHttpContext context = null) : this(null, className, methodName, context)
        {
        }

        public ServiceProxyInvocation(WebServiceProxyDescriptors webServiceProxyDescriptors, string className, string methodName, IHttpContext context = null)
        {
            this.WebServiceProxyDescriptors = webServiceProxyDescriptors ?? new WebServiceProxyDescriptors { WebServiceRegistry = WebServiceRegistry.ForApplicationServiceRegistry(ApplicationServiceRegistry.ForProcess()) };
            this.Context = context ?? new HttpContextWrapper();
            this.ClassName = className;
            this.MethodName = methodName;

            OnAnyInstanciated(this);
        }

        public static ServiceProxyInvocation Create(ServiceRegistry serviceRegistry, MethodInfo method, params ServiceProxyInvocationArgument[] arguments)
        {
            ServiceProxyInvocation request = new ServiceProxyInvocation()
            {
                ServiceRegistry = serviceRegistry,
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
            validation.Execute(Context);//, InputString);
            return validation;
        }

        ILogger _logger;
        public ILogger Logger
        {
            get => _logger ?? Log.Default;
            set => _logger = value;
        }

        IApiKeyResolver _apiKeyResolver;
        readonly object _apiKeyResolverSync = new object();
        public IApiKeyResolver ApiKeyResolver
        {
            get
            {
                return _apiKeyResolverSync.DoubleCheckLock(ref _apiKeyResolver, () => new ApiKeyResolver());
            }
            set => _apiKeyResolver = value;
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

        ServiceRegistry _serviceRegistry;
        public ServiceRegistry ServiceRegistry
        {
            get
            {
                if(_serviceRegistry == null)
                {
                    if(WebServiceProxyDescriptors != null)
                    {
                        _serviceRegistry = WebServiceProxyDescriptors.WebServiceRegistry;
                    }
                }

                return _serviceRegistry;
            }
            set
            {
                _serviceRegistry = value;
            }
        }

        Type _targetType;
        public Type TargetType
        {
            get
            {
                if (_targetType == null && !string.IsNullOrWhiteSpace(ClassName))
                {
                    InvocationTarget = ServiceRegistry.Get(ClassName, out _targetType);
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
                    _invocationTarget = ServiceRegistry.Get(ClassName);
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
                    _parameterInfos = MethodInfo.GetParameters();
                }

                return _parameterInfos;
            }
        }

        public ServiceProxyInvocationArgument[] Arguments
        {
            get;
            set;
        }

        int _maxRecursion;
        public int MaxRecursion
        {
            get
            {
                if(_maxRecursion <= 0)
                {
                    _maxRecursion = DefaultMaxRecursion;
                }
                return _maxRecursion;
            }
            set
            {
                _maxRecursion = value;
            }
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

/*        public bool HasCallback => !string.IsNullOrEmpty(Request.QueryString["callback"]);
*/
/*        string _callBack;
        readonly object _callBackLock = new object();

        /// <summary>
        /// The name of the javascript client side callback function if any or "callback"
        /// </summary>
		public string Callback
        {
            get
            {
                if (string.IsNullOrEmpty(_callBack))
                {
                    lock (_callBackLock)
                    {
                        if (string.IsNullOrEmpty(_callBack))
                        {
                            _callBack = "callback";
                            if (Request != null)
                            {
                                string qCb = Request.QueryString["callback"];
                                if (!string.IsNullOrEmpty(qCb))
                                {
                                    _callBack = qCb;
                                }
                            }
                        }
                    }
                }

                return _callBack;
            }
            set => _callBack = value;
        }*/
/*
        public string ViewName { get; set; }*/

        /*        private string GetMessage(Exception ex, bool stack)
                {
                    string st = stack ? ex.StackTrace : "";
                    return $"{ex.Message}:\r\n\r\n{st}";
                }*/

/*        private object[] GetJsonArguments(string[] jsonStrings)
        {
            if (jsonStrings.Length != ParameterInfos.Length)
            {
                throw new TargetParameterCountException();
            }

            object[] paramInstances = new object[ParameterInfos.Length];
            for (int i = 0; i < ParameterInfos.Length; i++)
            {
                string paramJson = jsonStrings[i];
                Type paramType = ParameterInfos[i].ParameterType;
                paramInstances[i] = paramJson?.FromJson(paramType);

                SetDefault(paramInstances, i);
            }
            return paramInstances;
        }*/

        /*        private object[] GetNamedQueryStringArguments()
                {
                    object[] results = new object[ParameterInfos.Length];
                    for (int i = 0; i < ParameterInfos.Length; i++)
                    {
                        System.Reflection.ParameterInfo paramInfo = ParameterInfos[i];
                        Type paramType = paramInfo.ParameterType;
                        string value = Request.QueryString[paramInfo.Name];
                        SetValue(results, i, paramType, value);

                        SetDefault(results, i);
                    }

                    return results;
                }*/

/*        private void SetDefault(object[] parameters, int i)
        {
            object val = parameters[i];
            if (val == null && ParameterInfos[i].HasDefaultValue)
            {
                parameters[i] = ParameterInfos[i].DefaultValue;
            }
        }*/

        /*        private object[] GetNumberedQueryStringArguments()
                {
                    object[] results = new object[ParameterInfos.Length];
                    for (int i = 0; i < ParameterInfos.Length; i++)
                    {
                        Type paramType = ParameterInfos[i].ParameterType;
                        string value = WebUtility.UrlDecode(Request.QueryString[i.ToString()]);
                        SetValue(results, i, paramType, value);

                        SetDefault(results, i);
                    }

                    return results;
                }*/

        /*        private static void SetValue(object[] results, int i, Type paramType, string value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        results[i] = null;
                    }
                    else
                    {
                        if (paramType == typeof(string) ||
                           paramType == typeof(int) ||
                           paramType == typeof(decimal) ||
                           paramType == typeof(long))
                        {
                            results[i] = Convert.ChangeType(value, paramType);
                        }
                        else
                        {
                            results[i] = value.FromJson(paramType);
                        }
                    }
                }*/

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
        /*
                private void Reset()
                {
                    IsInitialized = false;
                    Result = null;
                }*/

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

        public event EventHandler<ServiceProxyInvocation> Initializing;
        protected void OnInitializing()
        {
            Initializing?.Invoke(this, this);
        }
        public event EventHandler<ServiceProxyInvocation> Initialized;
        protected void OnInitialized()
        {
            Initialized?.Invoke(this, this);
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

        // -- TODO: move these events to ServiceProxyInvocationResolver
        public event Action<ServiceProxyInvocation, object> ContextSet;
        protected void OnContextSet(object target)
        {
            ContextSet?.Invoke(this, target);
        }

        public event Action<ServiceProxyInvocation, object> ServiceRegistrySet;
        protected void OnServiceRegsitrySet(object target)
        {
            ServiceRegistrySet?.Invoke(this, target);
        }
        // -- / end TODO

        public bool ExecuteWithoutValidation()
        {
            return Execute(InvocationTarget, false);
        }

        public bool Execute()
        {
            return Execute(InvocationTarget, true);
        }

        public bool Execute(object target, bool validate = true)
        {
            bool result = false;
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
                    //Initialize();
                    target = SetContext(target);
                    target = SetServiceProvider(target);
                    OnAnyExecuting(target);
                    OnExecuting(target);
                    Result = MethodInfo.Invoke(target, Arguments.Select(arg => arg.Value).ToArray());
                    OnExecuted(target);
                    OnAnyExecuted(target);
                    result = true;
                }
                catch (Exception ex)
                {
                    string resultMessage = $"{ex.GetInnerException().Message} \r\n\r\n\t{ex.GetInnerException()?.StackTrace}";
                    Result = resultMessage;
                    result = false;
                    OnExecutionException(target);
                    OnAnyExecutionException(target);
                }
            }

            //WasExecuted = true;
            //Success = result;
            return result;
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

        protected internal object SetServiceProvider(object target)
        {
            object result = target;
            if (target is IHasServiceRegistry hasServiceRegistry)
            {
                hasServiceRegistry.ServiceRegistry = ServiceRegistry;
                OnServiceRegsitrySet(target);
                result = hasServiceRegistry;
            }
            return result;
        }
    }
}