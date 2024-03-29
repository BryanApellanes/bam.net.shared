using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Configuration;
using Bam.Net.Logging;
using System.IO;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;

namespace Bam.Net.Server
{
    public static class ServiceConfig
    {
        public static string ContentRoot => DefaultConfiguration.GetAppSetting("ContentRoot").Or(BamHome.ContentPath);

        public static string LogRoot => Path.Combine(ContentRoot, "logs");

        /// <summary>
        /// A logical identifier for the current process
        /// </summary>
        public static string ProcessName => DefaultConfiguration.GetAppSetting("ProcessName").Or(DefaultConfiguration.DefaultProcessName);

        public static Database GetConfiguredDatabase()
        {
            string dialect = DefaultConfiguration.GetAppSetting("SqlDialect");
            string connectionString = DefaultConfiguration.GetAppSetting("SqlConnectionString");

            if (!Enum.TryParse<SqlDialect>(dialect, out SqlDialect sqlDialect))
            {
                sqlDialect = SqlDialect.SQLite;
                connectionString = "Default";
            }

            return DatabaseFactory.Instance.GetDatabase(sqlDialect, connectionString);
        }

        public static HostBinding[] GetConfiguredHostBindings()
        {
            return HostBinding.FromDefaultConfiguration();
        }
        
        static ILogger multiTargetLogger;
        public static ILogger GetMultiTargetLogger(ILogger logger)
        {
            if (multiTargetLogger == null)
            {
                MultiTargetLogger multiLogger = new MultiTargetLogger();                
                multiLogger.AddLogger(logger);
                multiLogger.StartLoggingThread();
                multiTargetLogger = multiLogger;
            }
            return multiTargetLogger;
        }
    }
}
