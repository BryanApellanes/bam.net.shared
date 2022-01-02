/*
	This file was generated and should not be modified directly
*/
// model is SchemaDefinition
using System;
using System.Data;
using System.Data.Common;
using Bam.Net;
using Bam.Net.Data;
using Bam.Net.Data.Qi;

namespace Bam.Net.ServiceProxy.Data.Dao
{
	// schema = ServiceProxyData
    public static class ServiceProxyDataContext
    {
		public static string ConnectionName
		{
			get
			{
				return "ServiceProxyData";
			}
		}

		public static Database Db
		{
			get
			{
				return Bam.Net.Data.Db.For(ConnectionName);
			}
		}


	public class SecureChannelSessionQueryContext
	{
			public SecureChannelSessionCollection Where(WhereDelegate<SecureChannelSessionColumns> where, Database db = null)
			{
				return Bam.Net.ServiceProxy.Data.Dao.SecureChannelSession.Where(where, db);
			}
		   
			public SecureChannelSessionCollection Where(WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy = null, Database db = null)
			{
				return Bam.Net.ServiceProxy.Data.Dao.SecureChannelSession.Where(where, orderBy, db);
			}

			public SecureChannelSession OneWhere(WhereDelegate<SecureChannelSessionColumns> where, Database db = null)
			{
				return Bam.Net.ServiceProxy.Data.Dao.SecureChannelSession.OneWhere(where, db);
			}

			public static SecureChannelSession GetOneWhere(WhereDelegate<SecureChannelSessionColumns> where, Database db = null)
			{
				return Bam.Net.ServiceProxy.Data.Dao.SecureChannelSession.GetOneWhere(where, db);
			}
		
			public SecureChannelSession FirstOneWhere(WhereDelegate<SecureChannelSessionColumns> where, Database db = null)
			{
				return Bam.Net.ServiceProxy.Data.Dao.SecureChannelSession.FirstOneWhere(where, db);
			}

			public SecureChannelSessionCollection Top(int count, WhereDelegate<SecureChannelSessionColumns> where, Database db = null)
			{
				return Bam.Net.ServiceProxy.Data.Dao.SecureChannelSession.Top(count, where, db);
			}

			public SecureChannelSessionCollection Top(int count, WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy, Database db = null)
			{
				return Bam.Net.ServiceProxy.Data.Dao.SecureChannelSession.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<SecureChannelSessionColumns> where, Database db = null)
			{
				return Bam.Net.ServiceProxy.Data.Dao.SecureChannelSession.Count(where, db);
			}
	}

	static SecureChannelSessionQueryContext _secureChannelSessions;
	static object _secureChannelSessionsLock = new object();
	public static SecureChannelSessionQueryContext SecureChannelSessions
	{
		get
		{
			return _secureChannelSessionsLock.DoubleCheckLock<SecureChannelSessionQueryContext>(ref _secureChannelSessions, () => new SecureChannelSessionQueryContext());
		}
	}
    }
}																								
