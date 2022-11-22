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
    public class KeySetQuery: Query<KeySetColumns, KeySet>
    { 
		public KeySetQuery(){}
		public KeySetQuery(WhereDelegate<KeySetColumns> where, OrderBy<KeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public KeySetQuery(Func<KeySetColumns, QueryFilter<KeySetColumns>> where, OrderBy<KeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public KeySetQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static KeySetQuery Where(WhereDelegate<KeySetColumns> where)
        {
            return Where(where, null, null);
        }

        public static KeySetQuery Where(WhereDelegate<KeySetColumns> where, OrderBy<KeySetColumns> orderBy = null, Database db = null)
        {
            return new KeySetQuery(where, orderBy, db);
        }

		public KeySetCollection Execute()
		{
			return new KeySetCollection(this, true);
		}
    }
}