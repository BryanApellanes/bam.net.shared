﻿using System;
using System.Collections.Generic;
using Bam.Net.Incubation;
using Bam.Net.Messaging;
using Bam.Net.ServiceProxy;
using Bam.Net.UserAccounts;
using System.IO;
using Bam.Net.Data.Repositories;
using Bam.Net.Server;
using Bam.Net.Logging;
using Bam.Net.Data;
using Bam.Net.ServiceProxy.Secure;
using Bam.Net.CoreServices.Files;
using Bam.Net.CoreServices.AssemblyManagement.Data.Dao.Repository;
using Bam.Net.CoreServices.ServiceRegistration.Data.Dao.Repository;
using Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.Repository;
using Bam.Net.CoreServices;

namespace Bam.Net.Services.DataReplication
{
    /// <summary>
    /// Service container for DataReplication services
    /// </summary>
    [ServiceRegistryContainer]
    public static partial class JournalRegistryContainer
    {
        public const string RegistryName = "DataReplication";
        static readonly object __DataReplicationIncubatorLock = new object();
        static ServiceRegistry __DataReplicationServiceRegistry;

        static readonly Dictionary<ProcessModes, Func<ServiceRegistry>> _factories;
        static JournalRegistryContainer()
        {
            _factories = new Dictionary<ProcessModes, Func<ServiceRegistry>>
            {
                { ProcessModes.Dev, CreateServicesRegistryForDev },
                { ProcessModes.Test, CreateServicesRegistryForTest },
                { ProcessModes.Prod, CreateServicesRegistryForProd }
            };
        }

        [ServiceRegistryLoader(RegistryName)]
        public static ServiceRegistry GetServiceRegistry()
        {
            return __DataReplicationIncubatorLock.DoubleCheckLock(ref __DataReplicationServiceRegistry, _factories[ProcessMode.Current.Mode]);
        }

        static ServiceRegistry _instance;
        static readonly object _instanceLock = new object();
        public static ServiceRegistry Instance => _instanceLock.DoubleCheckLock(ref _instance, Create);

        // place holders for customization if necessary

        [ServiceRegistryLoader(RegistryName, ProcessModes.Dev)]
        public static ServiceRegistry CreateServicesRegistryForDev()
        {
            ServiceRegistry registry = Create();

            // Add dev customizations here

            return registry;
        }

        [ServiceRegistryLoader(RegistryName, ProcessModes.Test)]
        public static ServiceRegistry CreateServicesRegistryForTest()
        {
            ServiceRegistry registry = Create();

            // Add test customizations here

            return registry;
        }

        [ServiceRegistryLoader(RegistryName, ProcessModes.Prod)]
        public static ServiceRegistry CreateServicesRegistryForProd()
        {
            ServiceRegistry registry = Create();

            // Add prod customizations here

            return registry;
        }
        // --
    }
}
