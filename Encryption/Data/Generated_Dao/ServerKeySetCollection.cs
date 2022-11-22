using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class ServerKeySetCollection: DaoCollection<ServerKeySetColumns, ServerKeySet>
    { 
		public ServerKeySetCollection(){}
		public ServerKeySetCollection(Database db, DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ServerKeySetCollection(DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(table, dao, rc) { }
		public ServerKeySetCollection(Query<ServerKeySetColumns, ServerKeySet> q, Bam.Net.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ServerKeySetCollection(Database db, Query<ServerKeySetColumns, ServerKeySet> q, bool load) : base(db, q, load) { }
		public ServerKeySetCollection(Query<ServerKeySetColumns, ServerKeySet> q, bool load) : base(q, load) { }
    }
}