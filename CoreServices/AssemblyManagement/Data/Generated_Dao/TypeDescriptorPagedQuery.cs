/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.CoreServices.AssemblyManagement.Data.Dao
{
    public class TypeDescriptorPagedQuery: PagedQuery<TypeDescriptorColumns, TypeDescriptor>
    { 
		public TypeDescriptorPagedQuery(TypeDescriptorColumns orderByColumn,TypeDescriptorQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}