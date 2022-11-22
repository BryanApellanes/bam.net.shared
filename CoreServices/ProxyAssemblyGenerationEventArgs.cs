/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Reflection;

namespace Bam.Net.CoreServices
{
    public class ProxyAssemblyGenerationEventArgs
    {
        /// <summary>
        /// Gets or sets Type of the service being generated or null .
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the settings used to generate or null.
        /// </summary>
        public ProxySettings ProxySettings { get; set; }

        /// <summary>
        /// Gets or sets the MethodInfo being reported as non virtual or null.
        /// </summary>
        public MethodInfo NonVirtualMethod { get; set; }

        /// <summary>
        /// Gets or sets the assembly.
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// Gets or sets the Exception.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
