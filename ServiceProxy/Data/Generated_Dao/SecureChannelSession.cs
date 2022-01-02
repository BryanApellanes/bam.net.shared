/*
	This file was generated and should not be modified directly (handlebars template)
*/
// Model is Table
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Bam.Net;
using Bam.Net.Data;
using Bam.Net.Data.Qi;

namespace Bam.Net.ServiceProxy.Data.Dao
{
	// schema = ServiceProxyData
	// connection Name = ServiceProxyData
	[Serializable]
	[Bam.Net.Data.Table("SecureChannelSession", "ServiceProxyData")]
	public partial class SecureChannelSession: Bam.Net.Data.Dao
	{
		public SecureChannelSession():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public SecureChannelSession(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public SecureChannelSession(Database db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public SecureChannelSession(Database db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Net.Exclude]
		public static implicit operator SecureChannelSession(DataRow data)
		{
			return new SecureChannelSession(data);
		}

		private void SetChildren()
		{




		} // end SetChildren

	// property: Id, columnName: Id
	[Bam.Net.Exclude]
	[Bam.Net.Data.KeyColumn(Name="Id", DbDataType="BigInt", MaxLength="19")]
	public ulong? Id
	{
		get
		{
			return GetULongValue("Id");
		}
		set
		{
			SetValue("Id", value);
		}
	}
    // property:Uuid, columnName: Uuid	
    [Bam.Net.Data.Column(Name="Uuid", DbDataType="VarChar", MaxLength="4000", AllowNull=false)]
    public string Uuid
    {
        get
        {
            return GetStringValue("Uuid");
        }
        set
        {
            SetValue("Uuid", value);
        }
    }

    // property:Cuid, columnName: Cuid	
    [Bam.Net.Data.Column(Name="Cuid", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Cuid
    {
        get
        {
            return GetStringValue("Cuid");
        }
        set
        {
            SetValue("Cuid", value);
        }
    }

    // property:Identifier, columnName: Identifier	
    [Bam.Net.Data.Column(Name="Identifier", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Identifier
    {
        get
        {
            return GetStringValue("Identifier");
        }
        set
        {
            SetValue("Identifier", value);
        }
    }

    // property:AsymmetricKey, columnName: AsymmetricKey	
    [Bam.Net.Data.Column(Name="AsymmetricKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string AsymmetricKey
    {
        get
        {
            return GetStringValue("AsymmetricKey");
        }
        set
        {
            SetValue("AsymmetricKey", value);
        }
    }

    // property:SymmetricKey, columnName: SymmetricKey	
    [Bam.Net.Data.Column(Name="SymmetricKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string SymmetricKey
    {
        get
        {
            return GetStringValue("SymmetricKey");
        }
        set
        {
            SetValue("SymmetricKey", value);
        }
    }

    // property:SymmetricIV, columnName: SymmetricIV	
    [Bam.Net.Data.Column(Name="SymmetricIV", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string SymmetricIV
    {
        get
        {
            return GetStringValue("SymmetricIV");
        }
        set
        {
            SetValue("SymmetricIV", value);
        }
    }

    // property:TimeOffset, columnName: TimeOffset	
    [Bam.Net.Data.Column(Name="TimeOffset", DbDataType="Int", MaxLength="10", AllowNull=true)]
    public int? TimeOffset
    {
        get
        {
            return GetIntValue("TimeOffset");
        }
        set
        {
            SetValue("TimeOffset", value);
        }
    }

    // property:LastActivity, columnName: LastActivity	
    [Bam.Net.Data.Column(Name="LastActivity", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? LastActivity
    {
        get
        {
            return GetDateTimeValue("LastActivity");
        }
        set
        {
            SetValue("LastActivity", value);
        }
    }

    // property:Expires, columnName: Expires	
    [Bam.Net.Data.Column(Name="Expires", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? Expires
    {
        get
        {
            return GetDateTimeValue("Expires");
        }
        set
        {
            SetValue("Expires", value);
        }
    }

    // property:Server, columnName: Server	
    [Bam.Net.Data.Column(Name="Server", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Server
    {
        get
        {
            return GetStringValue("Server");
        }
        set
        {
            SetValue("Server", value);
        }
    }

    // property:Client, columnName: Client	
    [Bam.Net.Data.Column(Name="Client", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Client
    {
        get
        {
            return GetStringValue("Client");
        }
        set
        {
            SetValue("Client", value);
        }
    }

    // property:Key, columnName: Key	
    [Bam.Net.Data.Column(Name="Key", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? Key
    {
        get
        {
            return GetULongValue("Key");
        }
        set
        {
            SetValue("Key", value);
        }
    }

    // property:CompositeKeyId, columnName: CompositeKeyId	
    [Bam.Net.Data.Column(Name="CompositeKeyId", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? CompositeKeyId
    {
        get
        {
            return GetULongValue("CompositeKeyId");
        }
        set
        {
            SetValue("CompositeKeyId", value);
        }
    }

    // property:CompositeKey, columnName: CompositeKey	
    [Bam.Net.Data.Column(Name="CompositeKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string CompositeKey
    {
        get
        {
            return GetStringValue("CompositeKey");
        }
        set
        {
            SetValue("CompositeKey", value);
        }
    }

    // property:CreatedBy, columnName: CreatedBy	
    [Bam.Net.Data.Column(Name="CreatedBy", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string CreatedBy
    {
        get
        {
            return GetStringValue("CreatedBy");
        }
        set
        {
            SetValue("CreatedBy", value);
        }
    }

    // property:ModifiedBy, columnName: ModifiedBy	
    [Bam.Net.Data.Column(Name="ModifiedBy", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string ModifiedBy
    {
        get
        {
            return GetStringValue("ModifiedBy");
        }
        set
        {
            SetValue("ModifiedBy", value);
        }
    }

    // property:Modified, columnName: Modified	
    [Bam.Net.Data.Column(Name="Modified", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? Modified
    {
        get
        {
            return GetDateTimeValue("Modified");
        }
        set
        {
            SetValue("Modified", value);
        }
    }

    // property:Deleted, columnName: Deleted	
    [Bam.Net.Data.Column(Name="Deleted", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? Deleted
    {
        get
        {
            return GetDateTimeValue("Deleted");
        }
        set
        {
            SetValue("Deleted", value);
        }
    }

    // property:Created, columnName: Created	
    [Bam.Net.Data.Column(Name="Created", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? Created
    {
        get
        {
            return GetDateTimeValue("Created");
        }
        set
        {
            SetValue("Created", value);
        }
    }







		/// <summary>
        /// Gets a query filter that should uniquely identify
        /// the current instance.  The default implementation
        /// compares the Id/key field to the current instance's.
        /// </summary>
		[Bam.Net.Exclude]
		public override IQueryFilter GetUniqueFilter()
		{
			if(UniqueFilterProvider != null)
			{
				return UniqueFilterProvider(this);
			}
			else
			{
				var colFilter = new SecureChannelSessionColumns();
				return (colFilter.KeyColumn == GetId());
			}
		}

		/// <summary>
        /// Return every record in the SecureChannelSession table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static SecureChannelSessionCollection LoadAll(Database database = null)
		{
			Database db = database ?? Db.For<SecureChannelSession>();
            SqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<SecureChannelSession>();
            var results = new SecureChannelSessionCollection(db, sql.GetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Net.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<SecureChannelSession>> batchProcessor, Database database = null)
		{
			await Task.Run(async ()=>
			{
				SecureChannelSessionColumns columns = new SecureChannelSessionColumns();
				var orderBy = Bam.Net.Data.Order.By<SecureChannelSessionColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, (c) => c.KeyColumn > 0, orderBy, database);
				while(results.Count > 0)
				{
					await Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (c) => c.KeyColumn > topId, orderBy, database);
				}
			});
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery(int batchSize, QueryFilter filter, Action<IEnumerable<SecureChannelSession>> batchProcessor, Database database = null)
		{
			await BatchQuery(batchSize, (c) => filter, batchProcessor, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery(int batchSize, WhereDelegate<SecureChannelSessionColumns> where, Action<IEnumerable<SecureChannelSession>> batchProcessor, Database database = null)
		{
			await Task.Run(async ()=>
			{
				SecureChannelSessionColumns columns = new SecureChannelSessionColumns();
				var orderBy = Bam.Net.Data.Order.By<SecureChannelSessionColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (SecureChannelSessionColumns)where(columns) && columns.KeyColumn > topId, orderBy, database);
				}
			});
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, QueryFilter filter, Action<IEnumerable<SecureChannelSession>> batchProcessor, Bam.Net.Data.OrderBy<SecureChannelSessionColumns> orderBy, Database database = null)
		{
			await BatchQuery<ColType>(batchSize, (c) => filter, batchProcessor, orderBy, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, WhereDelegate<SecureChannelSessionColumns> where, Action<IEnumerable<SecureChannelSession>> batchProcessor, Bam.Net.Data.OrderBy<SecureChannelSessionColumns> orderBy, Database database = null)
		{
			await Task.Run(async ()=>
			{
				SecureChannelSessionColumns columns = new SecureChannelSessionColumns();
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await Task.Run(()=>
					{
						batchProcessor(results);
					});
					ColType top = results.Select(d => d.Property<ColType>(orderBy.Column.ToString())).ToArray().Largest();
					results = Top(batchSize, (SecureChannelSessionColumns)where(columns) && orderBy.Column > top, orderBy, database);
				}
			});
		}

		public static SecureChannelSession GetById(uint? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified SecureChannelSession.Id was null");
			return GetById(id.Value, database);
		}

		public static SecureChannelSession GetById(uint id, Database database = null)
		{
			return GetById((ulong)id, database);
		}

		public static SecureChannelSession GetById(int? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified SecureChannelSession.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static SecureChannelSession GetById(int id, Database database = null)
		{
			return GetById((long)id, database);
		}

		public static SecureChannelSession GetById(long? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified SecureChannelSession.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static SecureChannelSession GetById(long id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static SecureChannelSession GetById(ulong? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified SecureChannelSession.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static SecureChannelSession GetById(ulong id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static SecureChannelSession GetByUuid(string uuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Uuid") == uuid, database);
		}

		public static SecureChannelSession GetByCuid(string cuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Query(QueryFilter filter, Database database = null)
		{
			return Where(filter, database);
		}

		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Where(QueryFilter filter, Database database = null)
		{
			WhereDelegate<SecureChannelSessionColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a SecureChannelSessionColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Where(Func<SecureChannelSessionColumns, QueryFilter<SecureChannelSessionColumns>> where, OrderBy<SecureChannelSessionColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<SecureChannelSession>();
			return new SecureChannelSessionCollection(database.GetQuery<SecureChannelSessionColumns, SecureChannelSession>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Where(WhereDelegate<SecureChannelSessionColumns> where, Database database = null)
		{
			database = database ?? Db.For<SecureChannelSession>();
			var results = new SecureChannelSessionCollection(database, database.GetQuery<SecureChannelSessionColumns, SecureChannelSession>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Where(WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<SecureChannelSession>();
			var results = new SecureChannelSessionCollection(database, database.GetQuery<SecureChannelSessionColumns, SecureChannelSession>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`SecureChannelSessionColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static SecureChannelSessionCollection Where(QiQuery where, Database database = null)
		{
			var results = new SecureChannelSessionCollection(database, Select<SecureChannelSessionColumns>.From<SecureChannelSession>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static SecureChannelSession GetOneWhere(QueryFilter where, Database database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				result = CreateFromFilter(where, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSession OneWhere(QueryFilter where, Database database = null)
		{
			WhereDelegate<SecureChannelSessionColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<SecureChannelSessionColumns> where, Database database = null)
		{
			SetOneWhere(where, out SecureChannelSession ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<SecureChannelSessionColumns> where, out SecureChannelSession result, Database database = null)
		{
			result = GetOneWhere(where, database);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSession GetOneWhere(WhereDelegate<SecureChannelSessionColumns> where, Database database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				SecureChannelSessionColumns c = new SecureChannelSessionColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single SecureChannelSession instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSession OneWhere(WhereDelegate<SecureChannelSessionColumns> where, Database database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`SecureChannelSessionColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static SecureChannelSession OneWhere(QiQuery where, Database database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSession FirstOneWhere(WhereDelegate<SecureChannelSessionColumns> where, Database database = null)
		{
			var results = Top(1, where, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSession FirstOneWhere(WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy, Database database = null)
		{
			var results = Top(1, where, orderBy, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Shortcut for Top(1, where, orderBy, database)
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSession FirstOneWhere(QueryFilter where, OrderBy<SecureChannelSessionColumns> orderBy = null, Database database = null)
		{
			WhereDelegate<SecureChannelSessionColumns> whereDelegate = (c) => where;
			var results = Top(1, whereDelegate, orderBy, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Execute a query and return the specified number
		/// of values. This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Top(int count, WhereDelegate<SecureChannelSessionColumns> where, Database database = null)
		{
			return Top(count, where, null, database);
		}

		/// <summary>
		/// Execute a query and return the specified number of values.  This method
		/// will issue a sql TOP clause so only the specified number of values
		/// will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Top(int count, WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy, Database database = null)
		{
			SecureChannelSessionColumns c = new SecureChannelSessionColumns();
			IQueryFilter filter = where(c);

			Database db = database ?? Db.For<SecureChannelSession>();
			QuerySet query = GetQuerySet(db);
			query.Top<SecureChannelSession>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<SecureChannelSessionColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<SecureChannelSessionCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Top(int count, QueryFilter where, Database database)
		{
			return Top(count, where, null, database);
		}
		/// <summary>
		/// Execute a query and return the specified number of values.  This method
		/// will issue a sql TOP clause so only the specified number of values
		/// will be returned.
		/// of values
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A QueryFilter used to filter the
		/// results
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Top(int count, QueryFilter where, OrderBy<SecureChannelSessionColumns> orderBy = null, Database database = null)
		{
			Database db = database ?? Db.For<SecureChannelSession>();
			QuerySet query = GetQuerySet(db);
			query.Top<SecureChannelSession>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<SecureChannelSessionColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<SecureChannelSessionCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static SecureChannelSessionCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, Database database = null)
		{
			Database db = database ?? Db.For<SecureChannelSession>();
			QuerySet query = GetQuerySet(db);
			query.Top<SecureChannelSession>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<SecureChannelSessionCollection>(0);
			results.Database = db;
			return results;
		}

		/// <summary>
		/// Execute a query and return the specified number of values.  This method
		/// will issue a sql TOP clause so only the specified number of values
		/// will be returned.
		/// of values
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A QueryFilter used to filter the
		/// results
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		public static SecureChannelSessionCollection Top(int count, QiQuery where, Database database = null)
		{
			Database db = database ?? Db.For<SecureChannelSession>();
			QuerySet query = GetQuerySet(db);
			query.Top<SecureChannelSession>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<SecureChannelSessionCollection>(0);
			results.Database = db;
			return results;
		}

		/// <summary>
		/// Return the count of @(Model.ClassName.Pluralize())
		/// </summary>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		public static long Count(Database database = null)
        {
			Database db = database ?? Db.For<SecureChannelSession>();
            QuerySet query = GetQuerySet(db);
            query.Count<SecureChannelSession>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a SecureChannelSessionColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static long Count(WhereDelegate<SecureChannelSessionColumns> where, Database database = null)
		{
			SecureChannelSessionColumns c = new SecureChannelSessionColumns();
			IQueryFilter filter = where(c) ;

			Database db = database ?? Db.For<SecureChannelSession>();
			QuerySet query = GetQuerySet(db);
			query.Count<SecureChannelSession>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, Database database = null)
		{
		    Database db = database ?? Db.For<SecureChannelSession>();
			QuerySet query = GetQuerySet(db);
			query.Count<SecureChannelSession>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static SecureChannelSession CreateFromFilter(IQueryFilter filter, Database database = null)
		{
			Database db = database ?? Db.For<SecureChannelSession>();
			var dao = new SecureChannelSession();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static SecureChannelSession OneOrThrow(SecureChannelSessionCollection c)
		{
			if(c.Count == 1)
			{
				return c[0];
			}
			else if(c.Count > 1)
			{
				throw new MultipleEntriesFoundException();
			}

			return null;
		}

	}
}
