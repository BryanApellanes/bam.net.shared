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
	[Bam.Net.Data.Table("WriteEvent", "DataReplication")]
	public partial class WriteEvent: Bam.Net.Data.Dao
	{
		public WriteEvent():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public WriteEvent(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public WriteEvent(Database db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public WriteEvent(Database db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Net.Exclude]
		public static implicit operator WriteEvent(DataRow data)
		{
			return new WriteEvent(data);
		}

		private void SetChildren()
		{


			if(_database != null)
			{
				this.ChildCollections.Add("DataProperty_WriteEventId", new DataPropertyCollection(Database.GetQuery<DataPropertyColumns, DataProperty>((c) => c.WriteEventId == GetULongValue("Id")), this, "WriteEventId"));
			}


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

	// property:Type, columnName: Type	
	[Bam.Net.Data.Column(Name="Type", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string Type
	{
		get
		{
			return GetStringValue("Type");
		}
		set
		{
			SetValue("Type", value);
		}
	}

	// property:InstanceCuid, columnName: InstanceCuid	
	[Bam.Net.Data.Column(Name="InstanceCuid", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string InstanceCuid
	{
		get
		{
			return GetStringValue("InstanceCuid");
		}
		set
		{
			SetValue("InstanceCuid", value);
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



	[Bam.Net.Exclude]	
	public DataPropertyCollection DataPropertiesByWriteEventId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
			}

			if(!this.ChildCollections.ContainsKey("DataProperty_WriteEventId"))
			{
				SetChildren();
			}

			var c = (DataPropertyCollection)this.ChildCollections["DataProperty_WriteEventId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
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
				var colFilter = new WriteEventColumns();
				return (colFilter.KeyColumn == IdValue);
			}
		}

		/// <summary>
        /// Return every record in the WriteEvent table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static WriteEventCollection LoadAll(Database database = null)
		{
			Database db = database ?? Db.For<WriteEvent>();
            SqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<WriteEvent>();
            var results = new WriteEventCollection(db, sql.GetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Net.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<WriteEvent>> batchProcessor, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				WriteEventColumns columns = new WriteEventColumns();
				var orderBy = Bam.Net.Data.Order.By<WriteEventColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
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
		public static async Task BatchQuery(int batchSize, QueryFilter filter, Action<IEnumerable<WriteEvent>> batchProcessor, Database database = null)
		{
			await BatchQuery(batchSize, (c) => filter, batchProcessor, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery(int batchSize, WhereDelegate<WriteEventColumns> where, Action<IEnumerable<WriteEvent>> batchProcessor, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				WriteEventColumns columns = new WriteEventColumns();
				var orderBy = Bam.Net.Data.Order.By<WriteEventColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (WriteEventColumns)where(columns) && columns.KeyColumn > topId, orderBy, database);
				}
			});
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, QueryFilter filter, Action<IEnumerable<WriteEvent>> batchProcessor, Bam.Net.Data.OrderBy<WriteEventColumns> orderBy, Database database = null)
		{
			await BatchQuery<ColType>(batchSize, (c) => filter, batchProcessor, orderBy, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, WhereDelegate<WriteEventColumns> where, Action<IEnumerable<WriteEvent>> batchProcessor, Bam.Net.Data.OrderBy<WriteEventColumns> orderBy, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				WriteEventColumns columns = new WriteEventColumns();
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
					{
						batchProcessor(results);
					});
					ColType top = results.Select(d => d.Property<ColType>(orderBy.Column.ToString())).ToArray().Largest();
					results = Top(batchSize, (WriteEventColumns)where(columns) && orderBy.Column > top, orderBy, database);
				}
			});
		}

		public static WriteEvent GetById(uint? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified WriteEvent.Id was null");
			return GetById(id.Value, database);
		}

		public static WriteEvent GetById(uint id, Database database = null)
		{
			return GetById((ulong)id, database);
		}

		public static WriteEvent GetById(int? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified WriteEvent.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static WriteEvent GetById(int id, Database database = null)
		{
			return GetById((long)id, database);
		}

		public static WriteEvent GetById(long? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified WriteEvent.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static WriteEvent GetById(long id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static WriteEvent GetById(ulong? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified WriteEvent.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static WriteEvent GetById(ulong id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static WriteEvent GetByUuid(string uuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Uuid") == uuid, database);
		}

		public static WriteEvent GetByCuid(string cuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Net.Exclude]
		public static WriteEventCollection Query(QueryFilter filter, Database database = null)
		{
			return Where(filter, database);
		}

		[Bam.Net.Exclude]
		public static WriteEventCollection Where(QueryFilter filter, Database database = null)
		{
			WhereDelegate<WriteEventColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a WriteEventColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static WriteEventCollection Where(Func<WriteEventColumns, QueryFilter<WriteEventColumns>> where, OrderBy<WriteEventColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<WriteEvent>();
			return new WriteEventCollection(database.GetQuery<WriteEventColumns, WriteEvent>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static WriteEventCollection Where(WhereDelegate<WriteEventColumns> where, Database database = null)
		{
			database = database ?? Db.For<WriteEvent>();
			var results = new WriteEventCollection(database, database.GetQuery<WriteEventColumns, WriteEvent>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static WriteEventCollection Where(WhereDelegate<WriteEventColumns> where, OrderBy<WriteEventColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<WriteEvent>();
			var results = new WriteEventCollection(database, database.GetQuery<WriteEventColumns, WriteEvent>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`WriteEventColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static WriteEventCollection Where(QiQuery where, Database database = null)
		{
			var results = new WriteEventCollection(database, Select<WriteEventColumns>.From<WriteEvent>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static WriteEvent GetOneWhere(QueryFilter where, Database database = null)
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
		public static WriteEvent OneWhere(QueryFilter where, Database database = null)
		{
			WhereDelegate<WriteEventColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<WriteEventColumns> where, Database database = null)
		{
			SetOneWhere(where, out WriteEvent ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<WriteEventColumns> where, out WriteEvent result, Database database = null)
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
		public static WriteEvent GetOneWhere(WhereDelegate<WriteEventColumns> where, Database database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				WriteEventColumns c = new WriteEventColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single WriteEvent instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static WriteEvent OneWhere(WhereDelegate<WriteEventColumns> where, Database database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`WriteEventColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static WriteEvent OneWhere(QiQuery where, Database database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static WriteEvent FirstOneWhere(WhereDelegate<WriteEventColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static WriteEvent FirstOneWhere(WhereDelegate<WriteEventColumns> where, OrderBy<WriteEventColumns> orderBy, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static WriteEvent FirstOneWhere(QueryFilter where, OrderBy<WriteEventColumns> orderBy = null, Database database = null)
		{
			WhereDelegate<WriteEventColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static WriteEventCollection Top(int count, WhereDelegate<WriteEventColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static WriteEventCollection Top(int count, WhereDelegate<WriteEventColumns> where, OrderBy<WriteEventColumns> orderBy, Database database = null)
		{
			WriteEventColumns c = new WriteEventColumns();
			IQueryFilter filter = where(c);

			Database db = database ?? Db.For<WriteEvent>();
			QuerySet query = GetQuerySet(db);
			query.Top<WriteEvent>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<WriteEventColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<WriteEventCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static WriteEventCollection Top(int count, QueryFilter where, Database database)
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
		public static WriteEventCollection Top(int count, QueryFilter where, OrderBy<WriteEventColumns> orderBy = null, Database database = null)
		{
			Database db = database ?? Db.For<WriteEvent>();
			QuerySet query = GetQuerySet(db);
			query.Top<WriteEvent>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<WriteEventColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<WriteEventCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static WriteEventCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, Database database = null)
		{
			Database db = database ?? Db.For<WriteEvent>();
			QuerySet query = GetQuerySet(db);
			query.Top<WriteEvent>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<WriteEventCollection>(0);
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
		public static WriteEventCollection Top(int count, QiQuery where, Database database = null)
		{
			Database db = database ?? Db.For<WriteEvent>();
			QuerySet query = GetQuerySet(db);
			query.Top<WriteEvent>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<WriteEventCollection>(0);
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
			Database db = database ?? Db.For<WriteEvent>();
            QuerySet query = GetQuerySet(db);
            query.Count<WriteEvent>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a WriteEventColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between WriteEventColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static long Count(WhereDelegate<WriteEventColumns> where, Database database = null)
		{
			WriteEventColumns c = new WriteEventColumns();
			IQueryFilter filter = where(c) ;

			Database db = database ?? Db.For<WriteEvent>();
			QuerySet query = GetQuerySet(db);
			query.Count<WriteEvent>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, Database database = null)
		{
		    Database db = database ?? Db.For<WriteEvent>();
			QuerySet query = GetQuerySet(db);
			query.Count<WriteEvent>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static WriteEvent CreateFromFilter(IQueryFilter filter, Database database = null)
		{
			Database db = database ?? Db.For<WriteEvent>();
			var dao = new WriteEvent();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static WriteEvent OneOrThrow(WriteEventCollection c)
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
