/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net.Encryption;
using Bam.Net.Server.ServiceProxy;
using Bam.Net.ServiceProxy.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class ServiceProxyInvocationValidationResult
    {
        readonly ServiceProxyInvocation _serviceProxyInvocation;

        public ServiceProxyInvocationValidationResult()
        {
            this.Success = true;
        }

        public ServiceProxyInvocationValidationResult(ServiceProxyInvocation request, string messageDelimiter = null)
        {
            _serviceProxyInvocation = request;
            Delimiter = messageDelimiter ?? ",\r\n";
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public ValidationFailures[] ValidationFailures { get; set; }

        internal void Execute(IHttpContext context)//, string input)
        {
            List<ValidationFailures> failures = new List<ValidationFailures>();
            List<string> messages = new List<string>();

            // TODO: update ValidateEncryptedToken to read input from context.request.inputstream or whatever is available.
            //ValidateEncryptedToken(context, input, failures, messages);
            ValidateClassName(failures, messages);
            ValidateTargetType(failures, messages);
            ValidateMethodName(failures, messages);
            ValidateMethodExists(failures, messages);
            ValidateLocalExcludeMethod(context, failures, messages);
            ValidateParameterCount(failures, messages);
            ValidateMethodRoles(context, failures, messages);
            ValidateClassRoles(context, failures, messages);
            //ValidateHmac(failures, messages);
            ValidateRequestFilters(context, failures, messages);

            ValidationFailures = failures.ToArray();
            Message = messages.ToArray().ToDelimited(s => s, Delimiter);
            this.Success = failures.Count == 0;
        }

        private void ValidateRequestFilters(IHttpContext context, List<ValidationFailures> failures, List<string> messages)
        {
            if (_serviceProxyInvocation.TargetType != null &&
                _serviceProxyInvocation.MethodInfo != null &&
                (
                    _serviceProxyInvocation.TargetType.HasCustomAttributeOfType(true, out RequestFilterAttribute filterAttr) ||
                    _serviceProxyInvocation.MethodInfo.HasCustomAttributeOfType(true, out filterAttr)
                ))
            {
                if (!filterAttr.RequestIsAllowed(_serviceProxyInvocation, out string failureMessage))
                {
                    failures.Add(ServiceProxy.ValidationFailures.AttributeFilterFailed);
                    messages.Add(failureMessage);
                }
            }
        }

/*        private void ValidateHmac(List<ValidationFailures> failures, List<string> messages)
        {
            ApiHmacKeyRequiredAttribute keyRequired;
            if (_toValidate.TargetType != null &&
                _toValidate.MethodInfo != null &&
                (
                    _toValidate.TargetType.HasCustomAttributeOfType(true, out keyRequired) ||
                    _toValidate.MethodInfo.HasCustomAttributeOfType(true, out keyRequired)
                ))
            {
                IApiHmacKeyResolver resolver = _toValidate.ApiKeyResolver;
*//*                if (!resolver.IsValidRequest(_toValidate))
                {
                    failures.Add(ServiceProxy.ValidationFailures.InvalidApiKeyToken);
                    messages.Add("ApiSigningKeyValidation failed");
                }*//*
            }
        }*/

        private void ValidateClassRoles(IHttpContext context, List<ValidationFailures> failures, List<string> messages)
        {
            if (_serviceProxyInvocation.TargetType != null &&
                _serviceProxyInvocation.TargetType.HasCustomAttributeOfType(true, out RoleRequiredAttribute requiredClassRoles))
            {
                if (requiredClassRoles.Roles.Length > 0)
                {
                    CheckRoles(failures, messages, requiredClassRoles, context);
                }
            }
        }

        private void ValidateMethodRoles(IHttpContext context, List<ValidationFailures> failures, List<string> messages)
        {
            if (_serviceProxyInvocation.TargetType != null &&
                            _serviceProxyInvocation.MethodInfo != null &&
                            _serviceProxyInvocation.MethodInfo.HasCustomAttributeOfType(true, out RoleRequiredAttribute requiredMethodRoles))
            {
                if (requiredMethodRoles.Roles.Length > 0)
                {
                    CheckRoles(failures, messages, requiredMethodRoles, context);
                }
            }
        }

        private void ValidateParameterCount(List<ValidationFailures> failures, List<string> messages)
        {
            if (_serviceProxyInvocation.ParameterInfos != null && _serviceProxyInvocation.ParameterInfos.Length != _serviceProxyInvocation.Arguments.Length)
            {
                failures.Add(ServiceProxy.ValidationFailures.ParameterCountMismatch);
                messages.Add("Wrong number of parameters specified: expected ({0}), recieved ({1})"._Format(_serviceProxyInvocation.ParameterInfos.Length, _serviceProxyInvocation.Arguments.Length));
            }
        }

        private void ValidateLocalExcludeMethod(IHttpContext context, List<ValidationFailures> failures, List<string> messages)
        {
            if (_serviceProxyInvocation.TargetType != null &&
                            _serviceProxyInvocation.MethodInfo != null &&
                            _serviceProxyInvocation.MethodInfo.HasCustomAttributeOfType(out ExcludeAttribute attr))
            {
                if (attr is LocalAttribute)
                {
                    if (!context.Request.UserHostAddress.StartsWith("127.0.0.1"))
                    {
                        failures.Add(ServiceProxy.ValidationFailures.RemoteExecutionNotAllowed);
                        messages.Add("The specified method is marked [Local] and the request was directed to {0}: {1}"._Format(context.Request.UserHostAddress, _serviceProxyInvocation.MethodName));
                    }
                }
                else
                {
                    failures.Add(ServiceProxy.ValidationFailures.MethodNotProxied);
                    messages.Add("The specified method is explicitly excluded from being proxied: {0}"._Format(_serviceProxyInvocation.MethodName));
                }
            }
        }

        private void ValidateMethodExists(List<ValidationFailures> failures, List<string> messages)
        {
            if (_serviceProxyInvocation.TargetType != null && _serviceProxyInvocation.MethodInfo == null)
            {
                failures.Add(ServiceProxy.ValidationFailures.MethodNotFound);
                string message = "Method ({0}) was not found"._Format(_serviceProxyInvocation.MethodName);
                if (!failures.Contains(ServiceProxy.ValidationFailures.ClassNameNotSpecified))
                {
                    message = "{0} on class ({1})"._Format(message, _serviceProxyInvocation.ClassName);
                }
                messages.Add(message);
            }
        }

        private void ValidateMethodName(List<ValidationFailures> failures, List<string> messages)
        {
            if (string.IsNullOrWhiteSpace(_serviceProxyInvocation.MethodName))
            {
                failures.Add(ServiceProxy.ValidationFailures.MethodNameNotSpecified);
                messages.Add("MethodName not specified");
            }
        }

        private void ValidateTargetType(List<ValidationFailures> failures, List<string> messages)
        {
            if (_serviceProxyInvocation.TargetType == null)
            {
                failures.Add(ServiceProxy.ValidationFailures.ClassNotRegistered);
                messages.Add("Class {0} was not registered as a proxied service.  Register the class with the ServiceProxySystem first."._Format(_serviceProxyInvocation.ClassName));
            }
        }

        private void ValidateClassName(List<ValidationFailures> failures, List<string> messages)
        {
            if (string.IsNullOrWhiteSpace(_serviceProxyInvocation.ClassName))
            {
                failures.Add(ServiceProxy.ValidationFailures.ClassNameNotSpecified);
                messages.Add("ClassName not specified");
            }
        }

        private static void ValidateEncryptedToken(IHttpContext context, string input, List<ValidationFailures> failures, List<string> messages)
        {
            if (input != null)
            {
                try
                {
                    EncryptedTokenValidationStatus tokenStatus = ApiValidation.ValidateEncryptedToken(context, input);
                    switch (tokenStatus)
                    {
                        case EncryptedTokenValidationStatus.Unknown:
                            failures.Add(ServiceProxy.ValidationFailures.UnknownTokenValidationResult);
                            messages.Add("ApiEncryptionValidation.ValidateToken failed");
                            break;
                        case EncryptedTokenValidationStatus.HashFailed:
                            failures.Add(ServiceProxy.ValidationFailures.TokenHashFailed);
                            messages.Add("ApiEncryptionValidation.ValidateToken failed: TokenHashFailed");
                            break;
                        case EncryptedTokenValidationStatus.NonceFailed:
                            failures.Add(ServiceProxy.ValidationFailures.TokenNonceFailed);
                            messages.Add("ApiEncryptionValidation.ValidateToken failed: TokenNonceFailed");
                            break;
                        case EncryptedTokenValidationStatus.Success:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    failures.Add(ServiceProxy.ValidationFailures.TokenValidationError);
                    messages.Add(ex.Message);
                }
            }
        }

        private static void CheckRoles(List<ValidationFailures> failures, List<string> messages, RoleRequiredAttribute requiredRoles, IHttpContext context)
        {
            IUserResolver userResolver = (IUserResolver)ServiceProxySystem.UserResolvers.Clone();
            IRoleResolver roleResolver = (IRoleResolver)ServiceProxySystem.RoleResolvers.Clone();
            userResolver.HttpContext = context;
            roleResolver.HttpContext = context;
            List<string> userRoles = new List<string>(roleResolver.GetRoles(userResolver));
            bool passed = false;
            for (int i = 0; i < requiredRoles.Roles.Length; i++)
            {
                string requiredRole = requiredRoles.Roles[i];
                if (userRoles.Contains(requiredRole))
                {
                    passed = true;
                    break;
                }
            }

            if (!passed)
            {
                failures.Add(ServiceProxy.ValidationFailures.PermissionDenied);
                messages.Add("Permission Denied");
            }
        }

        internal string Delimiter { get; set; }
    }
}
