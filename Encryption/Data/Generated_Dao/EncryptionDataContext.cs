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

namespace Bam.Net.Encryption.Data.Dao
{
	// schema = EncryptionData
    public static class EncryptionDataContext
    {
		public static string ConnectionName
		{
			get
			{
				return "EncryptionData";
			}
		}

		public static Database Db
		{
			get
			{
				return Bam.Net.Data.Db.For(ConnectionName);
			}
		}


	public class ClientKeySetQueryContext
	{
			public ClientKeySetCollection Where(WhereDelegate<ClientKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ClientKeySet.Where(where, db);
			}
		   
			public ClientKeySetCollection Where(WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy = null, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ClientKeySet.Where(where, orderBy, db);
			}

			public ClientKeySet OneWhere(WhereDelegate<ClientKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ClientKeySet.OneWhere(where, db);
			}

			public static ClientKeySet GetOneWhere(WhereDelegate<ClientKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ClientKeySet.GetOneWhere(where, db);
			}
		
			public ClientKeySet FirstOneWhere(WhereDelegate<ClientKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ClientKeySet.FirstOneWhere(where, db);
			}

			public ClientKeySetCollection Top(int count, WhereDelegate<ClientKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ClientKeySet.Top(count, where, db);
			}

			public ClientKeySetCollection Top(int count, WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ClientKeySet.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ClientKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ClientKeySet.Count(where, db);
			}
	}

	static ClientKeySetQueryContext _clientKeySets;
	static object _clientKeySetsLock = new object();
	public static ClientKeySetQueryContext ClientKeySets
	{
		get
		{
			return _clientKeySetsLock.DoubleCheckLock<ClientKeySetQueryContext>(ref _clientKeySets, () => new ClientKeySetQueryContext());
		}
	}
	public class KeySetQueryContext
	{
			public KeySetCollection Where(WhereDelegate<KeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.KeySet.Where(where, db);
			}
		   
			public KeySetCollection Where(WhereDelegate<KeySetColumns> where, OrderBy<KeySetColumns> orderBy = null, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.KeySet.Where(where, orderBy, db);
			}

			public KeySet OneWhere(WhereDelegate<KeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.KeySet.OneWhere(where, db);
			}

			public static KeySet GetOneWhere(WhereDelegate<KeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.KeySet.GetOneWhere(where, db);
			}
		
			public KeySet FirstOneWhere(WhereDelegate<KeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.KeySet.FirstOneWhere(where, db);
			}

			public KeySetCollection Top(int count, WhereDelegate<KeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.KeySet.Top(count, where, db);
			}

			public KeySetCollection Top(int count, WhereDelegate<KeySetColumns> where, OrderBy<KeySetColumns> orderBy, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.KeySet.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<KeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.KeySet.Count(where, db);
			}
	}

	static KeySetQueryContext _keySets;
	static object _keySetsLock = new object();
	public static KeySetQueryContext KeySets
	{
		get
		{
			return _keySetsLock.DoubleCheckLock<KeySetQueryContext>(ref _keySets, () => new KeySetQueryContext());
		}
	}
	public class ServerKeySetQueryContext
	{
			public ServerKeySetCollection Where(WhereDelegate<ServerKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ServerKeySet.Where(where, db);
			}
		   
			public ServerKeySetCollection Where(WhereDelegate<ServerKeySetColumns> where, OrderBy<ServerKeySetColumns> orderBy = null, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ServerKeySet.Where(where, orderBy, db);
			}

			public ServerKeySet OneWhere(WhereDelegate<ServerKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ServerKeySet.OneWhere(where, db);
			}

			public static ServerKeySet GetOneWhere(WhereDelegate<ServerKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ServerKeySet.GetOneWhere(where, db);
			}
		
			public ServerKeySet FirstOneWhere(WhereDelegate<ServerKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ServerKeySet.FirstOneWhere(where, db);
			}

			public ServerKeySetCollection Top(int count, WhereDelegate<ServerKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ServerKeySet.Top(count, where, db);
			}

			public ServerKeySetCollection Top(int count, WhereDelegate<ServerKeySetColumns> where, OrderBy<ServerKeySetColumns> orderBy, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ServerKeySet.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ServerKeySetColumns> where, Database db = null)
			{
				return Bam.Net.Encryption.Data.Dao.ServerKeySet.Count(where, db);
			}
	}

	static ServerKeySetQueryContext _serverKeySets;
	static object _serverKeySetsLock = new object();
	public static ServerKeySetQueryContext ServerKeySets
	{
		get
		{
			return _serverKeySetsLock.DoubleCheckLock<ServerKeySetQueryContext>(ref _serverKeySets, () => new ServerKeySetQueryContext());
		}
	}
    }
}																								
