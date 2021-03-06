﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Incubation;
using Bam.Net.Data.Repositories;

namespace Bam.Net.CoreServices
{
    public partial class ServiceRegistry: Incubator
    {
        public string Name { get; set; }

        public FluentCtorContext<I> ForCtor<I>(string parameterName)
        {
            return new FluentCtorContext<I>(this, parameterName);
        }

        public FluentServiceRegistryContext<I> For<I>()
        {
            return new FluentServiceRegistryContext<I>(this);
        }

        public ServiceRegistry Include(Incubator incubator)
        {
            CombineWith(incubator, true);
            return this;
        }

        public ServiceRegistry Include(ServiceRegistry registry)
        {
            CombineWith(registry, true);
            return this;
        }

        public static ServiceRegistry Create()
        {
            return new ServiceRegistry();
        }

        public void Validate()
        {
            ValidateClassNames();
            ValidateClassTypes();
        }

        public void ValidateClassNames()
        {
            foreach (string className in ClassNames)
            {
                object instance = Get(className);
                Expect.IsNotNull(instance, $"{className} was null");
            }
        }

        public void ValidateClassTypes()
        {
            foreach (Type type in ClassNameTypes)
            {
                object instance = this[type];
                Expect.IsNotNull(instance, $"{type.Name} returned null");
            }
        }

        public new static ServiceRegistry Default { get; set; }
        public static Func<object> GetServiceLoader(Type type, object orDefault = null)
        {
            if (Default == null)
            {
                Type coreRegistryContainer = type.Assembly.GetTypes().FirstOrDefault(t => t.HasCustomAttributeOfType<ServiceRegistryContainerAttribute>());
                if (coreRegistryContainer != null)
                {
                    MethodInfo provider = coreRegistryContainer.GetMethods().FirstOrDefault(mi => CustomAttributeExtension.HasCustomAttributeOfType<ServiceRegistryLoaderAttribute>(mi) || mi.Name.Equals("Get"));
                    if (provider != null)
                    {
                        object instance = provider.IsStatic ? null : provider.DeclaringType.Construct();
                        Default = (ServiceRegistry)provider.Invoke(instance, null);
                    }
                }
            }
            return Default == null ? (() => type.Construct()) : (Func<object>)(() =>
            {
                if (!Default.TryGet(type, out object result))
                {
                    result = orDefault;
                }
                return result;
            });
        }
    }
}
