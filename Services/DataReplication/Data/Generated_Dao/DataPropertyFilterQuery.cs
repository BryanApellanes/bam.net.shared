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
    public class DataPropertyFilterQuery: Query<DataPropertyFilterColumns, DataPropertyFilter>
    { 
		public DataPropertyFilterQuery(){}
		public DataPropertyFilterQuery(WhereDelegate<DataPropertyFilterColumns> where, OrderBy<DataPropertyFilterColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public DataPropertyFilterQuery(Func<DataPropertyFilterColumns, QueryFilter<DataPropertyFilterColumns>> where, OrderBy<DataPropertyFilterColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public DataPropertyFilterQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static DataPropertyFilterQuery Where(WhereDelegate<DataPropertyFilterColumns> where)
        {
            return Where(where, null, null);
        }

        public static DataPropertyFilterQuery Where(WhereDelegate<DataPropertyFilterColumns> where, OrderBy<DataPropertyFilterColumns> orderBy = null, Database db = null)
        {
            return new DataPropertyFilterQuery(where, orderBy, db);
        }

		public DataPropertyFilterCollection Execute()
		{
			return new DataPropertyFilterCollection(this, true);
		}
    }
}