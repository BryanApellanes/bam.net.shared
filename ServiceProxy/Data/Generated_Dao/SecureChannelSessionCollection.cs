using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Net.Data;

namespace Bam.Net.ServiceProxy.Data.Dao
{
    public class SecureChannelSessionCollection: DaoCollection<SecureChannelSessionColumns, SecureChannelSession>
    { 
		public SecureChannelSessionCollection(){}
		public SecureChannelSessionCollection(Database db, DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public SecureChannelSessionCollection(DataTable table, Bam.Net.Data.Dao dao = null, string rc = null) : base(table, dao, rc) { }
		public SecureChannelSessionCollection(Query<SecureChannelSessionColumns, SecureChannelSession> q, Bam.Net.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public SecureChannelSessionCollection(Database db, Query<SecureChannelSessionColumns, SecureChannelSession> q, bool load) : base(db, q, load) { }
		public SecureChannelSessionCollection(Query<SecureChannelSessionColumns, SecureChannelSession> q, bool load) : base(q, load) { }
    }
}