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

namespace Bam.Net.Encryption.Data.Dao
{
	// schema = EncryptionData
	// connection Name = EncryptionData
	[Serializable]
	[Bam.Net.Data.Table("ClientKeySet", "EncryptionData")]
	public partial class ClientKeySet: Bam.Net.Data.Dao
	{
		public ClientKeySet():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public ClientKeySet(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public ClientKeySet(Database db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public ClientKeySet(Database db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Net.Exclude]
		public static implicit operator ClientKeySet(DataRow data)
		{
			return new ClientKeySet(data);
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

    // property:MachineName, columnName: MachineName	
    [Bam.Net.Data.Column(Name="MachineName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string MachineName
    {
        get
        {
            return GetStringValue("MachineName");
        }
        set
        {
            SetValue("MachineName", value);
        }
    }

    // property:ClientHostName, columnName: ClientHostName	
    [Bam.Net.Data.Column(Name="ClientHostName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string ClientHostName
    {
        get
        {
            return GetStringValue("ClientHostName");
        }
        set
        {
            SetValue("ClientHostName", value);
        }
    }

    // property:ServerHostName, columnName: ServerHostName	
    [Bam.Net.Data.Column(Name="ServerHostName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string ServerHostName
    {
        get
        {
            return GetStringValue("ServerHostName");
        }
        set
        {
            SetValue("ServerHostName", value);
        }
    }

    // property:PublicKey, columnName: PublicKey	
    [Bam.Net.Data.Column(Name="PublicKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string PublicKey
    {
        get
        {
            return GetStringValue("PublicKey");
        }
        set
        {
            SetValue("PublicKey", value);
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

    // property:AesKey, columnName: AesKey	
    [Bam.Net.Data.Column(Name="AesKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string AesKey
    {
        get
        {
            return GetStringValue("AesKey");
        }
        set
        {
            SetValue("AesKey", value);
        }
    }

    // property:AesIV, columnName: AesIV	
    [Bam.Net.Data.Column(Name="AesIV", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string AesIV
    {
        get
        {
            return GetStringValue("AesIV");
        }
        set
        {
            SetValue("AesIV", value);
        }
    }

    // property:ApplicationName, columnName: ApplicationName	
    [Bam.Net.Data.Column(Name="ApplicationName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string ApplicationName
    {
        get
        {
            return GetStringValue("ApplicationName");
        }
        set
        {
            SetValue("ApplicationName", value);
        }
    }

    // property:Secret, columnName: Secret	
    [Bam.Net.Data.Column(Name="Secret", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Secret
    {
        get
        {
            return GetStringValue("Secret");
        }
        set
        {
            SetValue("Secret", value);
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
				var colFilter = new ClientKeySetColumns();
				return (colFilter.KeyColumn == GetId());
			}
		}

		/// <summary>
        /// Return every record in the ClientKeySet table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static ClientKeySetCollection LoadAll(Database database = null)
		{
			Database db = database ?? Db.For<ClientKeySet>();
            SqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<ClientKeySet>();
            var results = new ClientKeySetCollection(db, sql.GetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Net.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<ClientKeySet>> batchProcessor, Database database = null)
		{
			await Task.Run(async ()=>
			{
				ClientKeySetColumns columns = new ClientKeySetColumns();
				var orderBy = Bam.Net.Data.Order.By<ClientKeySetColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
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
		public static async Task BatchQuery(int batchSize, QueryFilter filter, Action<IEnumerable<ClientKeySet>> batchProcessor, Database database = null)
		{
			await BatchQuery(batchSize, (c) => filter, batchProcessor, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery(int batchSize, WhereDelegate<ClientKeySetColumns> where, Action<IEnumerable<ClientKeySet>> batchProcessor, Database database = null)
		{
			await Task.Run(async ()=>
			{
				ClientKeySetColumns columns = new ClientKeySetColumns();
				var orderBy = Bam.Net.Data.Order.By<ClientKeySetColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (ClientKeySetColumns)where(columns) && columns.KeyColumn > topId, orderBy, database);
				}
			});
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, QueryFilter filter, Action<IEnumerable<ClientKeySet>> batchProcessor, Bam.Net.Data.OrderBy<ClientKeySetColumns> orderBy, Database database = null)
		{
			await BatchQuery<ColType>(batchSize, (c) => filter, batchProcessor, orderBy, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, WhereDelegate<ClientKeySetColumns> where, Action<IEnumerable<ClientKeySet>> batchProcessor, Bam.Net.Data.OrderBy<ClientKeySetColumns> orderBy, Database database = null)
		{
			await Task.Run(async ()=>
			{
				ClientKeySetColumns columns = new ClientKeySetColumns();
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await Task.Run(()=>
					{
						batchProcessor(results);
					});
					ColType top = results.Select(d => d.Property<ColType>(orderBy.Column.ToString())).ToArray().Largest();
					results = Top(batchSize, (ClientKeySetColumns)where(columns) && orderBy.Column > top, orderBy, database);
				}
			});
		}

		public static ClientKeySet GetById(uint? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified ClientKeySet.Id was null");
			return GetById(id.Value, database);
		}

		public static ClientKeySet GetById(uint id, Database database = null)
		{
			return GetById((ulong)id, database);
		}

		public static ClientKeySet GetById(int? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified ClientKeySet.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static ClientKeySet GetById(int id, Database database = null)
		{
			return GetById((long)id, database);
		}

		public static ClientKeySet GetById(long? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified ClientKeySet.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static ClientKeySet GetById(long id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static ClientKeySet GetById(ulong? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified ClientKeySet.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static ClientKeySet GetById(ulong id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static ClientKeySet GetByUuid(string uuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Uuid") == uuid, database);
		}

		public static ClientKeySet GetByCuid(string cuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Net.Exclude]
		public static ClientKeySetCollection Query(QueryFilter filter, Database database = null)
		{
			return Where(filter, database);
		}

		[Bam.Net.Exclude]
		public static ClientKeySetCollection Where(QueryFilter filter, Database database = null)
		{
			WhereDelegate<ClientKeySetColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a ClientKeySetColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static ClientKeySetCollection Where(Func<ClientKeySetColumns, QueryFilter<ClientKeySetColumns>> where, OrderBy<ClientKeySetColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<ClientKeySet>();
			return new ClientKeySetCollection(database.GetQuery<ClientKeySetColumns, ClientKeySet>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static ClientKeySetCollection Where(WhereDelegate<ClientKeySetColumns> where, Database database = null)
		{
			database = database ?? Db.For<ClientKeySet>();
			var results = new ClientKeySetCollection(database, database.GetQuery<ClientKeySetColumns, ClientKeySet>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static ClientKeySetCollection Where(WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<ClientKeySet>();
			var results = new ClientKeySetCollection(database, database.GetQuery<ClientKeySetColumns, ClientKeySet>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`ClientKeySetColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static ClientKeySetCollection Where(QiQuery where, Database database = null)
		{
			var results = new ClientKeySetCollection(database, Select<ClientKeySetColumns>.From<ClientKeySet>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static ClientKeySet GetOneWhere(QueryFilter where, Database database = null)
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
		public static ClientKeySet OneWhere(QueryFilter where, Database database = null)
		{
			WhereDelegate<ClientKeySetColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<ClientKeySetColumns> where, Database database = null)
		{
			SetOneWhere(where, out ClientKeySet ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<ClientKeySetColumns> where, out ClientKeySet result, Database database = null)
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
		public static ClientKeySet GetOneWhere(WhereDelegate<ClientKeySetColumns> where, Database database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				ClientKeySetColumns c = new ClientKeySetColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single ClientKeySet instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static ClientKeySet OneWhere(WhereDelegate<ClientKeySetColumns> where, Database database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`ClientKeySetColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static ClientKeySet OneWhere(QiQuery where, Database database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static ClientKeySet FirstOneWhere(WhereDelegate<ClientKeySetColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static ClientKeySet FirstOneWhere(WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static ClientKeySet FirstOneWhere(QueryFilter where, OrderBy<ClientKeySetColumns> orderBy = null, Database database = null)
		{
			WhereDelegate<ClientKeySetColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static ClientKeySetCollection Top(int count, WhereDelegate<ClientKeySetColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static ClientKeySetCollection Top(int count, WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy, Database database = null)
		{
			ClientKeySetColumns c = new ClientKeySetColumns();
			IQueryFilter filter = where(c);

			Database db = database ?? Db.For<ClientKeySet>();
			QuerySet query = GetQuerySet(db);
			query.Top<ClientKeySet>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<ClientKeySetColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<ClientKeySetCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static ClientKeySetCollection Top(int count, QueryFilter where, Database database)
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
		public static ClientKeySetCollection Top(int count, QueryFilter where, OrderBy<ClientKeySetColumns> orderBy = null, Database database = null)
		{
			Database db = database ?? Db.For<ClientKeySet>();
			QuerySet query = GetQuerySet(db);
			query.Top<ClientKeySet>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<ClientKeySetColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<ClientKeySetCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static ClientKeySetCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, Database database = null)
		{
			Database db = database ?? Db.For<ClientKeySet>();
			QuerySet query = GetQuerySet(db);
			query.Top<ClientKeySet>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<ClientKeySetCollection>(0);
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
		public static ClientKeySetCollection Top(int count, QiQuery where, Database database = null)
		{
			Database db = database ?? Db.For<ClientKeySet>();
			QuerySet query = GetQuerySet(db);
			query.Top<ClientKeySet>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<ClientKeySetCollection>(0);
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
			Database db = database ?? Db.For<ClientKeySet>();
            QuerySet query = GetQuerySet(db);
            query.Count<ClientKeySet>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ClientKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static long Count(WhereDelegate<ClientKeySetColumns> where, Database database = null)
		{
			ClientKeySetColumns c = new ClientKeySetColumns();
			IQueryFilter filter = where(c) ;

			Database db = database ?? Db.For<ClientKeySet>();
			QuerySet query = GetQuerySet(db);
			query.Count<ClientKeySet>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, Database database = null)
		{
		    Database db = database ?? Db.For<ClientKeySet>();
			QuerySet query = GetQuerySet(db);
			query.Count<ClientKeySet>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static ClientKeySet CreateFromFilter(IQueryFilter filter, Database database = null)
		{
			Database db = database ?? Db.For<ClientKeySet>();
			var dao = new ClientKeySet();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static ClientKeySet OneOrThrow(ClientKeySetCollection c)
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
