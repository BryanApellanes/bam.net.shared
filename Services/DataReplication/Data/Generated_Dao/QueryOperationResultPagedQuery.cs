/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.Services.DataReplication.Data.Dao
{
    public class QueryOperationResultPagedQuery: PagedQuery<QueryOperationResultColumns, QueryOperationResult>
    { 
		public QueryOperationResultPagedQuery(QueryOperationResultColumns orderByColumn,QueryOperationResultQuery query, Database db = null) : base(orderByColumn, query, db) { }
    }
}