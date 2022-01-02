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
    public class ApplicationKeySetQuery: Query<ApplicationKeySetColumns, ApplicationKeySet>
    { 
		public ApplicationKeySetQuery(){}
		public ApplicationKeySetQuery(WhereDelegate<ApplicationKeySetColumns> where, OrderBy<ApplicationKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ApplicationKeySetQuery(Func<ApplicationKeySetColumns, QueryFilter<ApplicationKeySetColumns>> where, OrderBy<ApplicationKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ApplicationKeySetQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ApplicationKeySetQuery Where(WhereDelegate<ApplicationKeySetColumns> where)
        {
            return Where(where, null, null);
        }

        public static ApplicationKeySetQuery Where(WhereDelegate<ApplicationKeySetColumns> where, OrderBy<ApplicationKeySetColumns> orderBy = null, Database db = null)
        {
            return new ApplicationKeySetQuery(where, orderBy, db);
        }

		public ApplicationKeySetCollection Execute()
		{
			return new ApplicationKeySetCollection(this, true);
		}
    }
}