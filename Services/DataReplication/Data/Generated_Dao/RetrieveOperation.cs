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

namespace Bam.Net.Services.DataReplication.Data.Dao
{
	// schema = DataReplication
	// connection Name = DataReplication
	[Serializable]
	[Bam.Net.Data.Table("RetrieveOperation", "DataReplication")]
	public partial class RetrieveOperation: Bam.Net.Data.Dao
	{
		public RetrieveOperation():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public RetrieveOperation(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public RetrieveOperation(Database db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public RetrieveOperation(Database db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Net.Exclude]
		public static implicit operator RetrieveOperation(DataRow data)
		{
			return new RetrieveOperation(data);
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

	// property:TypeNamespace, columnName: TypeNamespace	
	[Bam.Net.Data.Column(Name="TypeNamespace", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string TypeNamespace
	{
		get
		{
			return GetStringValue("TypeNamespace");
		}
		set
		{
			SetValue("TypeNamespace", value);
		}
	}

	// property:TypeName, columnName: TypeName	
	[Bam.Net.Data.Column(Name="TypeName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string TypeName
	{
		get
		{
			return GetStringValue("TypeName");
		}
		set
		{
			SetValue("TypeName", value);
		}
	}

	// property:AssemblyPath, columnName: AssemblyPath	
	[Bam.Net.Data.Column(Name="AssemblyPath", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string AssemblyPath
	{
		get
		{
			return GetStringValue("AssemblyPath");
		}
		set
		{
			SetValue("AssemblyPath", value);
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
				var colFilter = new RetrieveOperationColumns();
				return (colFilter.KeyColumn == IdValue);
			}
		}

		/// <summary>
        /// Return every record in the RetrieveOperation table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static RetrieveOperationCollection LoadAll(Database database = null)
		{
			Database db = database ?? Db.For<RetrieveOperation>();
            SqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<RetrieveOperation>();
            var results = new RetrieveOperationCollection(db, sql.GetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Net.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<RetrieveOperation>> batchProcessor, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				RetrieveOperationColumns columns = new RetrieveOperationColumns();
				var orderBy = Bam.Net.Data.Order.By<RetrieveOperationColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
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
		public static async Task BatchQuery(int batchSize, QueryFilter filter, Action<IEnumerable<RetrieveOperation>> batchProcessor, Database database = null)
		{
			await BatchQuery(batchSize, (c) => filter, batchProcessor, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery(int batchSize, WhereDelegate<RetrieveOperationColumns> where, Action<IEnumerable<RetrieveOperation>> batchProcessor, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				RetrieveOperationColumns columns = new RetrieveOperationColumns();
				var orderBy = Bam.Net.Data.Order.By<RetrieveOperationColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (RetrieveOperationColumns)where(columns) && columns.KeyColumn > topId, orderBy, database);
				}
			});
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, QueryFilter filter, Action<IEnumerable<RetrieveOperation>> batchProcessor, Bam.Net.Data.OrderBy<RetrieveOperationColumns> orderBy, Database database = null)
		{
			await BatchQuery<ColType>(batchSize, (c) => filter, batchProcessor, orderBy, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, WhereDelegate<RetrieveOperationColumns> where, Action<IEnumerable<RetrieveOperation>> batchProcessor, Bam.Net.Data.OrderBy<RetrieveOperationColumns> orderBy, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				RetrieveOperationColumns columns = new RetrieveOperationColumns();
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
					{
						batchProcessor(results);
					});
					ColType top = results.Select(d => d.Property<ColType>(orderBy.Column.ToString())).ToArray().Largest();
					results = Top(batchSize, (RetrieveOperationColumns)where(columns) && orderBy.Column > top, orderBy, database);
				}
			});
		}

		public static RetrieveOperation GetById(uint? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified RetrieveOperation.Id was null");
			return GetById(id.Value, database);
		}

		public static RetrieveOperation GetById(uint id, Database database = null)
		{
			return GetById((ulong)id, database);
		}

		public static RetrieveOperation GetById(int? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified RetrieveOperation.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static RetrieveOperation GetById(int id, Database database = null)
		{
			return GetById((long)id, database);
		}

		public static RetrieveOperation GetById(long? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified RetrieveOperation.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static RetrieveOperation GetById(long id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static RetrieveOperation GetById(ulong? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified RetrieveOperation.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static RetrieveOperation GetById(ulong id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static RetrieveOperation GetByUuid(string uuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Uuid") == uuid, database);
		}

		public static RetrieveOperation GetByCuid(string cuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Query(QueryFilter filter, Database database = null)
		{
			return Where(filter, database);
		}

		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Where(QueryFilter filter, Database database = null)
		{
			WhereDelegate<RetrieveOperationColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a RetrieveOperationColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Where(Func<RetrieveOperationColumns, QueryFilter<RetrieveOperationColumns>> where, OrderBy<RetrieveOperationColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<RetrieveOperation>();
			return new RetrieveOperationCollection(database.GetQuery<RetrieveOperationColumns, RetrieveOperation>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Where(WhereDelegate<RetrieveOperationColumns> where, Database database = null)
		{
			database = database ?? Db.For<RetrieveOperation>();
			var results = new RetrieveOperationCollection(database, database.GetQuery<RetrieveOperationColumns, RetrieveOperation>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Where(WhereDelegate<RetrieveOperationColumns> where, OrderBy<RetrieveOperationColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<RetrieveOperation>();
			var results = new RetrieveOperationCollection(database, database.GetQuery<RetrieveOperationColumns, RetrieveOperation>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`RetrieveOperationColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static RetrieveOperationCollection Where(QiQuery where, Database database = null)
		{
			var results = new RetrieveOperationCollection(database, Select<RetrieveOperationColumns>.From<RetrieveOperation>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static RetrieveOperation GetOneWhere(QueryFilter where, Database database = null)
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
		public static RetrieveOperation OneWhere(QueryFilter where, Database database = null)
		{
			WhereDelegate<RetrieveOperationColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<RetrieveOperationColumns> where, Database database = null)
		{
			SetOneWhere(where, out RetrieveOperation ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<RetrieveOperationColumns> where, out RetrieveOperation result, Database database = null)
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
		public static RetrieveOperation GetOneWhere(WhereDelegate<RetrieveOperationColumns> where, Database database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				RetrieveOperationColumns c = new RetrieveOperationColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single RetrieveOperation instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static RetrieveOperation OneWhere(WhereDelegate<RetrieveOperationColumns> where, Database database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`RetrieveOperationColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static RetrieveOperation OneWhere(QiQuery where, Database database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static RetrieveOperation FirstOneWhere(WhereDelegate<RetrieveOperationColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static RetrieveOperation FirstOneWhere(WhereDelegate<RetrieveOperationColumns> where, OrderBy<RetrieveOperationColumns> orderBy, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static RetrieveOperation FirstOneWhere(QueryFilter where, OrderBy<RetrieveOperationColumns> orderBy = null, Database database = null)
		{
			WhereDelegate<RetrieveOperationColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Top(int count, WhereDelegate<RetrieveOperationColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Top(int count, WhereDelegate<RetrieveOperationColumns> where, OrderBy<RetrieveOperationColumns> orderBy, Database database = null)
		{
			RetrieveOperationColumns c = new RetrieveOperationColumns();
			IQueryFilter filter = where(c);

			Database db = database ?? Db.For<RetrieveOperation>();
			QuerySet query = GetQuerySet(db);
			query.Top<RetrieveOperation>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<RetrieveOperationColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<RetrieveOperationCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Top(int count, QueryFilter where, Database database)
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
		public static RetrieveOperationCollection Top(int count, QueryFilter where, OrderBy<RetrieveOperationColumns> orderBy = null, Database database = null)
		{
			Database db = database ?? Db.For<RetrieveOperation>();
			QuerySet query = GetQuerySet(db);
			query.Top<RetrieveOperation>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<RetrieveOperationColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<RetrieveOperationCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static RetrieveOperationCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, Database database = null)
		{
			Database db = database ?? Db.For<RetrieveOperation>();
			QuerySet query = GetQuerySet(db);
			query.Top<RetrieveOperation>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<RetrieveOperationCollection>(0);
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
		public static RetrieveOperationCollection Top(int count, QiQuery where, Database database = null)
		{
			Database db = database ?? Db.For<RetrieveOperation>();
			QuerySet query = GetQuerySet(db);
			query.Top<RetrieveOperation>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<RetrieveOperationCollection>(0);
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
			Database db = database ?? Db.For<RetrieveOperation>();
            QuerySet query = GetQuerySet(db);
            query.Count<RetrieveOperation>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a RetrieveOperationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RetrieveOperationColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static long Count(WhereDelegate<RetrieveOperationColumns> where, Database database = null)
		{
			RetrieveOperationColumns c = new RetrieveOperationColumns();
			IQueryFilter filter = where(c) ;

			Database db = database ?? Db.For<RetrieveOperation>();
			QuerySet query = GetQuerySet(db);
			query.Count<RetrieveOperation>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, Database database = null)
		{
		    Database db = database ?? Db.For<RetrieveOperation>();
			QuerySet query = GetQuerySet(db);
			query.Count<RetrieveOperation>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static RetrieveOperation CreateFromFilter(IQueryFilter filter, Database database = null)
		{
			Database db = database ?? Db.For<RetrieveOperation>();
			var dao = new RetrieveOperation();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static RetrieveOperation OneOrThrow(RetrieveOperationCollection c)
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
