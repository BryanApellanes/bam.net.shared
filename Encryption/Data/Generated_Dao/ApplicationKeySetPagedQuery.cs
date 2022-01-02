/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class ApplicationKeySetPagedQuery: PagedQuery<ApplicationKeySetColumns, ApplicationKeySet>
    { 
		public ApplicationKeySetPagedQuery(ApplicationKeySetColumns orderByColumn,ApplicationKeySetQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}