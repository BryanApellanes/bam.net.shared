/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.CoreServices.ServiceRegistration.Data.Dao
{
    public class ServiceRegistryDescriptorPagedQuery: PagedQuery<ServiceRegistryDescriptorColumns, ServiceRegistryDescriptor>
    { 
		public ServiceRegistryDescriptorPagedQuery(ServiceRegistryDescriptorColumns orderByColumn, ServiceRegistryDescriptorQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}