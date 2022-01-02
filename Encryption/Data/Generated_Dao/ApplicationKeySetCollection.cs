using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class ApplicationKeySetCollection: DaoCollection<ApplicationKeySetColumns, ApplicationKeySet>
    { 
		public ApplicationKeySetCollection(){}
		public ApplicationKeySetCollection(Database db, DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ApplicationKeySetCollection(DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(table, dao, rc) { }
		public ApplicationKeySetCollection(Query<ApplicationKeySetColumns, ApplicationKeySet> q, Bam.Net.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ApplicationKeySetCollection(Database db, Query<ApplicationKeySetColumns, ApplicationKeySet> q, bool load) : base(db, q, load) { }
		public ApplicationKeySetCollection(Query<ApplicationKeySetColumns, ApplicationKeySet> q, bool load) : base(q, load) { }
    }
}