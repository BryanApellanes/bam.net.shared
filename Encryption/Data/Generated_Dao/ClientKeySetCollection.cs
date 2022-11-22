using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class ClientKeySetCollection: DaoCollection<ClientKeySetColumns, ClientKeySet>
    { 
		public ClientKeySetCollection(){}
		public ClientKeySetCollection(Database db, DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ClientKeySetCollection(DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(table, dao, rc) { }
		public ClientKeySetCollection(Query<ClientKeySetColumns, ClientKeySet> q, Bam.Net.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ClientKeySetCollection(Database db, Query<ClientKeySetColumns, ClientKeySet> q, bool load) : base(db, q, load) { }
		public ClientKeySetCollection(Query<ClientKeySetColumns, ClientKeySet> q, bool load) : base(q, load) { }
    }
}