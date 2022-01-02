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
    public class ServerKeySetQuery: Query<ServerKeySetColumns, ServerKeySet>
    { 
		public ServerKeySetQuery(){}
		public ServerKeySetQuery(WhereDelegate<ServerKeySetColumns> where, OrderBy<ServerKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ServerKeySetQuery(Func<ServerKeySetColumns, QueryFilter<ServerKeySetColumns>> where, OrderBy<ServerKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ServerKeySetQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ServerKeySetQuery Where(WhereDelegate<ServerKeySetColumns> where)
        {
            return Where(where, null, null);
        }

        public static ServerKeySetQuery Where(WhereDelegate<ServerKeySetColumns> where, OrderBy<ServerKeySetColumns> orderBy = null, Database db = null)
        {
            return new ServerKeySetQuery(where, orderBy, db);
        }

		public ServerKeySetCollection Execute()
		{
			return new ServerKeySetCollection(this, true);
		}
    }
}