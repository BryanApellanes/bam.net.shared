﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Services
{
    /// <summary>
    /// Used to addorn a class that should be added to a net core service as a scoped dependency collection by a call to RegisterAppModules.
    /// Use AppModuleAttribute if registration in the ApplicationServiceRegistry is all that is required by your application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScopedAttribute: AppServiceAttribute
    {
    }
}
