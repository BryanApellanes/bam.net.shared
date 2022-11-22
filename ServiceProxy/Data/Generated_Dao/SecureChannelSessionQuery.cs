/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.ServiceProxy.Data.Dao
{
    public class SecureChannelSessionQuery: Query<SecureChannelSessionColumns, SecureChannelSession>
    { 
		public SecureChannelSessionQuery(){}
		public SecureChannelSessionQuery(WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public SecureChannelSessionQuery(Func<SecureChannelSessionColumns, QueryFilter<SecureChannelSessionColumns>> where, OrderBy<SecureChannelSessionColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public SecureChannelSessionQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static SecureChannelSessionQuery Where(WhereDelegate<SecureChannelSessionColumns> where)
        {
            return Where(where, null, null);
        }

        public static SecureChannelSessionQuery Where(WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy = null, Database db = null)
        {
            return new SecureChannelSessionQuery(where, orderBy, db);
        }

		public SecureChannelSessionCollection Execute()
		{
			return new SecureChannelSessionCollection(this, true);
		}
    }
}