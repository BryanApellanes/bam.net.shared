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

namespace Bam.Net.CoreServices.Auth.Data.Dao
{
	// schema = Auth
	// connection Name = Auth
	[Serializable]
	[Bam.Net.Data.Table("AccessToken", "Auth")]
	public partial class AccessToken: Bam.Net.Data.Dao
	{
		public AccessToken():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AccessToken(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AccessToken(Database db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AccessToken(Database db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Net.Exclude]
		public static implicit operator AccessToken(DataRow data)
		{
			return new AccessToken(data);
		}

		private void SetChildren()
		{




		} // end SetChildren

	// property:Id, columnName: Id	
	[Bam.Net.Data.Column(Name="Id", DbDataType="BigInt", MaxLength="19", AllowNull=false)]
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

	// property:ProviderId, columnName: ProviderId	
	[Bam.Net.Data.Column(Name="ProviderId", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string ProviderId
	{
		get
		{
			return GetStringValue("ProviderId");
		}
		set
		{
			SetValue("ProviderId", value);
		}
	}

	// property:ProviderName, columnName: ProviderName	
	[Bam.Net.Data.Column(Name="ProviderName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string ProviderName
	{
		get
		{
			return GetStringValue("ProviderName");
		}
		set
		{
			SetValue("ProviderName", value);
		}
	}

	// property:Value, columnName: Value	
	[Bam.Net.Data.Column(Name="Value", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string Value
	{
		get
		{
			return GetStringValue("Value");
		}
		set
		{
			SetValue("Value", value);
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
				var colFilter = new AccessTokenColumns();
				return (colFilter.KeyColumn == IdValue);
			}
		}

		/// <summary>
        /// Return every record in the AccessToken table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static AccessTokenCollection LoadAll(Database database = null)
		{
			Database db = database ?? Db.For<AccessToken>();
            SqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<AccessToken>();
            var results = new AccessTokenCollection(db, sql.GetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Net.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<AccessToken>> batchProcessor, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				AccessTokenColumns columns = new AccessTokenColumns();
				var orderBy = Bam.Net.Data.Order.By<AccessTokenColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, (c) => c.KeyColumn > 0, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
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
		public static async Task BatchQuery(int batchSize, QueryFilter filter, Action<IEnumerable<AccessToken>> batchProcessor, Database database = null)
		{
			await BatchQuery(batchSize, (c) => filter, batchProcessor, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery(int batchSize, WhereDelegate<AccessTokenColumns> where, Action<IEnumerable<AccessToken>> batchProcessor, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				AccessTokenColumns columns = new AccessTokenColumns();
				var orderBy = Bam.Net.Data.Order.By<AccessTokenColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (AccessTokenColumns)where(columns) && columns.KeyColumn > topId, orderBy, database);
				}
			});
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, QueryFilter filter, Action<IEnumerable<AccessToken>> batchProcessor, Bam.Net.Data.OrderBy<AccessTokenColumns> orderBy, Database database = null)
		{
			await BatchQuery<ColType>(batchSize, (c) => filter, batchProcessor, orderBy, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, WhereDelegate<AccessTokenColumns> where, Action<IEnumerable<AccessToken>> batchProcessor, Bam.Net.Data.OrderBy<AccessTokenColumns> orderBy, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				AccessTokenColumns columns = new AccessTokenColumns();
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
					{
						batchProcessor(results);
					});
					ColType top = results.Select(d => d.Property<ColType>(orderBy.Column.ToString())).ToArray().Largest();
					results = Top(batchSize, (AccessTokenColumns)where(columns) && orderBy.Column > top, orderBy, database);
				}
			});
		}

		public static AccessToken GetById(uint? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AccessToken.Id was null");
			return GetById(id.Value, database);
		}

		public static AccessToken GetById(uint id, Database database = null)
		{
			return GetById((ulong)id, database);
		}

		public static AccessToken GetById(int? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AccessToken.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static AccessToken GetById(int id, Database database = null)
		{
			return GetById((long)id, database);
		}

		public static AccessToken GetById(long? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AccessToken.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static AccessToken GetById(long id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static AccessToken GetById(ulong? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AccessToken.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static AccessToken GetById(ulong id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static AccessToken GetByUuid(string uuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Uuid") == uuid, database);
		}

		public static AccessToken GetByCuid(string cuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Net.Exclude]
		public static AccessTokenCollection Query(QueryFilter filter, Database database = null)
		{
			return Where(filter, database);
		}

		[Bam.Net.Exclude]
		public static AccessTokenCollection Where(QueryFilter filter, Database database = null)
		{
			WhereDelegate<AccessTokenColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a AccessTokenColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static AccessTokenCollection Where(Func<AccessTokenColumns, QueryFilter<AccessTokenColumns>> where, OrderBy<AccessTokenColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<AccessToken>();
			return new AccessTokenCollection(database.GetQuery<AccessTokenColumns, AccessToken>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static AccessTokenCollection Where(WhereDelegate<AccessTokenColumns> where, Database database = null)
		{
			database = database ?? Db.For<AccessToken>();
			var results = new AccessTokenCollection(database, database.GetQuery<AccessTokenColumns, AccessToken>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static AccessTokenCollection Where(WhereDelegate<AccessTokenColumns> where, OrderBy<AccessTokenColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<AccessToken>();
			var results = new AccessTokenCollection(database, database.GetQuery<AccessTokenColumns, AccessToken>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`AccessTokenColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static AccessTokenCollection Where(QiQuery where, Database database = null)
		{
			var results = new AccessTokenCollection(database, Select<AccessTokenColumns>.From<AccessToken>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static AccessToken GetOneWhere(QueryFilter where, Database database = null)
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
		public static AccessToken OneWhere(QueryFilter where, Database database = null)
		{
			WhereDelegate<AccessTokenColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<AccessTokenColumns> where, Database database = null)
		{
			SetOneWhere(where, out AccessToken ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<AccessTokenColumns> where, out AccessToken result, Database database = null)
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
		public static AccessToken GetOneWhere(WhereDelegate<AccessTokenColumns> where, Database database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				AccessTokenColumns c = new AccessTokenColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single AccessToken instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static AccessToken OneWhere(WhereDelegate<AccessTokenColumns> where, Database database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`AccessTokenColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static AccessToken OneWhere(QiQuery where, Database database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static AccessToken FirstOneWhere(WhereDelegate<AccessTokenColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static AccessToken FirstOneWhere(WhereDelegate<AccessTokenColumns> where, OrderBy<AccessTokenColumns> orderBy, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static AccessToken FirstOneWhere(QueryFilter where, OrderBy<AccessTokenColumns> orderBy = null, Database database = null)
		{
			WhereDelegate<AccessTokenColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static AccessTokenCollection Top(int count, WhereDelegate<AccessTokenColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static AccessTokenCollection Top(int count, WhereDelegate<AccessTokenColumns> where, OrderBy<AccessTokenColumns> orderBy, Database database = null)
		{
			AccessTokenColumns c = new AccessTokenColumns();
			IQueryFilter filter = where(c);

			Database db = database ?? Db.For<AccessToken>();
			QuerySet query = GetQuerySet(db);
			query.Top<AccessToken>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<AccessTokenColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<AccessTokenCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static AccessTokenCollection Top(int count, QueryFilter where, Database database)
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
		public static AccessTokenCollection Top(int count, QueryFilter where, OrderBy<AccessTokenColumns> orderBy = null, Database database = null)
		{
			Database db = database ?? Db.For<AccessToken>();
			QuerySet query = GetQuerySet(db);
			query.Top<AccessToken>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<AccessTokenColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<AccessTokenCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static AccessTokenCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, Database database = null)
		{
			Database db = database ?? Db.For<AccessToken>();
			QuerySet query = GetQuerySet(db);
			query.Top<AccessToken>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<AccessTokenCollection>(0);
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
		public static AccessTokenCollection Top(int count, QiQuery where, Database database = null)
		{
			Database db = database ?? Db.For<AccessToken>();
			QuerySet query = GetQuerySet(db);
			query.Top<AccessToken>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<AccessTokenCollection>(0);
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
			Database db = database ?? Db.For<AccessToken>();
            QuerySet query = GetQuerySet(db);
            query.Count<AccessToken>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AccessTokenColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccessTokenColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static long Count(WhereDelegate<AccessTokenColumns> where, Database database = null)
		{
			AccessTokenColumns c = new AccessTokenColumns();
			IQueryFilter filter = where(c) ;

			Database db = database ?? Db.For<AccessToken>();
			QuerySet query = GetQuerySet(db);
			query.Count<AccessToken>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, Database database = null)
		{
		    Database db = database ?? Db.For<AccessToken>();
			QuerySet query = GetQuerySet(db);
			query.Count<AccessToken>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static AccessToken CreateFromFilter(IQueryFilter filter, Database database = null)
		{
			Database db = database ?? Db.For<AccessToken>();
			var dao = new AccessToken();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static AccessToken OneOrThrow(AccessTokenCollection c)
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
