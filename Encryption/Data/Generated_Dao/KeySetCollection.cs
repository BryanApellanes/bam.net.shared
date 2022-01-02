using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class KeySetCollection: DaoCollection<KeySetColumns, KeySet>
    { 
		public KeySetCollection(){}
		public KeySetCollection(Database db, DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public KeySetCollection(DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(table, dao, rc) { }
		public KeySetCollection(Query<KeySetColumns, KeySet> q, Bam.Net.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public KeySetCollection(Database db, Query<KeySetColumns, KeySet> q, bool load) : base(db, q, load) { }
		public KeySetCollection(Query<KeySetColumns, KeySet> q, bool load) : base(q, load) { }
    }
}