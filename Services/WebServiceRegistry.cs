using Bam.Net.CoreServices;
using Bam.Net.Incubation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Bam.Net.Services
{
    /// <summary>
    /// A service registry used to construct instances of classes addorned with the custom attribute `ProxyAttribute`.
    /// </summary>
    public class WebServiceRegistry: ServiceRegistry
    {
        public WebServiceRegistry() { }
        
        public WebServiceRegistry(Incubator registry)
        {
            CopyWebServices(registry);
        }

        public WebServiceRegistry CopyWebServices(Incubator registry)
        {
            foreach (string className in registry.ClassNames)
            {
                object instance = registry.Get(className, out Type type);
                if (type != null && instance != null && type.HasCustomAttributeOfType<ProxyAttribute>())
                {
                    Set(type, instance);
                }
            }
            return this;
        }

        public static WebServiceRegistry ForApplicationServiceRegistry(ApplicationServiceRegistry applicationServiceRegistry)
        {
            WebServiceRegistry fromAssembly = FromEntryAssembly(applicationServiceRegistry);
            return fromAssembly.CopyWebServices(applicationServiceRegistry);
        }

        /// <summary>
        /// Gets a WebServiceRegistry that contains all types addorned with the ProxyAttribute from the entry assembly.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        /// <returns></returns>
        public static WebServiceRegistry FromEntryAssembly(ServiceRegistry serviceRegistry = null)
        {
            WebServiceRegistry webServiceRegistry = new WebServiceRegistry();
            foreach (Type type in Assembly.GetEntryAssembly()
                .GetTypes()
                .Where(type => type.HasCustomAttributeOfType(out ProxyAttribute proxyAttribute)).ToArray())
            {
                if (serviceRegistry != null)
                {
                    webServiceRegistry.Set(type, () => serviceRegistry.Get(type));
                }
                else
                {
                    webServiceRegistry.Set(type, type.Construct());
                }
            }
            return webServiceRegistry;
        }

        public static WebServiceRegistry FromRegistry(ServiceRegistry registry)
        {
            return FromIncubator(registry);
        }

        public static WebServiceRegistry FromIncubator(Incubator incubator)
        {
            WebServiceRegistry result = new WebServiceRegistry();
            result.CopyWebServices(incubator);
            return result;
        }

        public object Get(string className, ApplicationServiceRegistry applicationServiceRegistry)
        {
            return Get(className, applicationServiceRegistry, out _);
        }

        public object Get(string className, ApplicationServiceRegistry applicationServiceRegistry, out Type type)
        {
            type = this[className];
            if (type != null)
            {
                object result = this[type];
                if (result is Func<object> fn)
                {
                    return fn() ?? Get(type, applicationServiceRegistry.GetCtorParams(type));
                }
                else if (result is Func<Type, object> typeFn)
                {
                    return typeFn(type) ?? Get(type, applicationServiceRegistry.GetCtorParams(type));
                }
                else if (result == null)
                {
                    result = Get(type, applicationServiceRegistry.GetCtorParams(type));
                }
                return result;
            }

            return null;
        }
    }
}
