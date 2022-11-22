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
    public class ClientKeySetQuery: Query<ClientKeySetColumns, ClientKeySet>
    { 
		public ClientKeySetQuery(){}
		public ClientKeySetQuery(WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ClientKeySetQuery(Func<ClientKeySetColumns, QueryFilter<ClientKeySetColumns>> where, OrderBy<ClientKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ClientKeySetQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ClientKeySetQuery Where(WhereDelegate<ClientKeySetColumns> where)
        {
            return Where(where, null, null);
        }

        public static ClientKeySetQuery Where(WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy = null, Database db = null)
        {
            return new ClientKeySetQuery(where, orderBy, db);
        }

		public ClientKeySetCollection Execute()
		{
			return new ClientKeySetCollection(this, true);
		}
    }
}