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

namespace Bam.Net.Encryption
{
	// schema = Encryption
	// connection Name = Encryption
	[Serializable]
	[Bam.Net.Data.Table("VaultKey", "Encryption")]
	public partial class VaultKey: Bam.Net.Data.Dao
	{
		public VaultKey():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public VaultKey(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public VaultKey(Database db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public VaultKey(Database db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Net.Exclude]
		public static implicit operator VaultKey(DataRow data)
		{
			return new VaultKey(data);
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

    // property:RsaKey, columnName: RsaKey	
    [Bam.Net.Data.Column(Name="RsaKey", DbDataType="VarChar", MaxLength="4000", AllowNull=false)]
    public string RsaKey
    {
        get
        {
            return GetStringValue("RsaKey");
        }
        set
        {
            SetValue("RsaKey", value);
        }
    }

    // property:Password, columnName: Password	
    [Bam.Net.Data.Column(Name="Password", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Password
    {
        get
        {
            return GetStringValue("Password");
        }
        set
        {
            SetValue("Password", value);
        }
    }


	// start VaultId -> VaultId
	[Bam.Net.Data.ForeignKey(
        Table="VaultKey",
		Name="VaultId",
		DbDataType="BigInt",
		MaxLength="",
		AllowNull=true,
		ReferencedKey="Id",
		ReferencedTable="Vault",
		Suffix="1")]
	public ulong? VaultId
	{
		get
		{
			return GetULongValue("VaultId", false);
		}
		set
		{
			SetValue("VaultId", value, false);
		}
	}

    Vault _vaultOfVaultId;
	public Vault VaultOfVaultId
	{
		get
		{
			if(_vaultOfVaultId == null)
			{
				_vaultOfVaultId = Bam.Net.Encryption.Vault.OneWhere(c => c.KeyColumn == this.VaultId, this.Database);
			}
			return _vaultOfVaultId;
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
				var colFilter = new VaultKeyColumns();
				return (colFilter.KeyColumn == GetId());
			}
		}

		/// <summary>
        /// Return every record in the VaultKey table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static VaultKeyCollection LoadAll(Database database = null)
		{
			Database db = database ?? Db.For<VaultKey>();
            SqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<VaultKey>();
            var results = new VaultKeyCollection(db, sql.GetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Net.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<VaultKey>> batchProcessor, Database database = null)
		{
			await Task.Run(async ()=>
			{
				VaultKeyColumns columns = new VaultKeyColumns();
				var orderBy = Bam.Net.Data.Order.By<VaultKeyColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
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
		public static async Task BatchQuery(int batchSize, QueryFilter filter, Action<IEnumerable<VaultKey>> batchProcessor, Database database = null)
		{
			await BatchQuery(batchSize, (c) => filter, batchProcessor, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery(int batchSize, WhereDelegate<VaultKeyColumns> where, Action<IEnumerable<VaultKey>> batchProcessor, Database database = null)
		{
			await Task.Run(async ()=>
			{
				VaultKeyColumns columns = new VaultKeyColumns();
				var orderBy = Bam.Net.Data.Order.By<VaultKeyColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (VaultKeyColumns)where(columns) && columns.KeyColumn > topId, orderBy, database);
				}
			});
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, QueryFilter filter, Action<IEnumerable<VaultKey>> batchProcessor, Bam.Net.Data.OrderBy<VaultKeyColumns> orderBy, Database database = null)
		{
			await BatchQuery<ColType>(batchSize, (c) => filter, batchProcessor, orderBy, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, WhereDelegate<VaultKeyColumns> where, Action<IEnumerable<VaultKey>> batchProcessor, Bam.Net.Data.OrderBy<VaultKeyColumns> orderBy, Database database = null)
		{
			await Task.Run(async ()=>
			{
				VaultKeyColumns columns = new VaultKeyColumns();
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await Task.Run(()=>
					{
						batchProcessor(results);
					});
					ColType top = results.Select(d => d.Property<ColType>(orderBy.Column.ToString())).ToArray().Largest();
					results = Top(batchSize, (VaultKeyColumns)where(columns) && orderBy.Column > top, orderBy, database);
				}
			});
		}

		public static VaultKey GetById(uint? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified VaultKey.Id was null");
			return GetById(id.Value, database);
		}

		public static VaultKey GetById(uint id, Database database = null)
		{
			return GetById((ulong)id, database);
		}

		public static VaultKey GetById(int? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified VaultKey.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static VaultKey GetById(int id, Database database = null)
		{
			return GetById((long)id, database);
		}

		public static VaultKey GetById(long? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified VaultKey.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static VaultKey GetById(long id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static VaultKey GetById(ulong? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified VaultKey.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static VaultKey GetById(ulong id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static VaultKey GetByUuid(string uuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Uuid") == uuid, database);
		}

		public static VaultKey GetByCuid(string cuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Net.Exclude]
		public static VaultKeyCollection Query(QueryFilter filter, Database database = null)
		{
			return Where(filter, database);
		}

		[Bam.Net.Exclude]
		public static VaultKeyCollection Where(QueryFilter filter, Database database = null)
		{
			WhereDelegate<VaultKeyColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a VaultKeyColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static VaultKeyCollection Where(Func<VaultKeyColumns, QueryFilter<VaultKeyColumns>> where, OrderBy<VaultKeyColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<VaultKey>();
			return new VaultKeyCollection(database.GetQuery<VaultKeyColumns, VaultKey>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static VaultKeyCollection Where(WhereDelegate<VaultKeyColumns> where, Database database = null)
		{
			database = database ?? Db.For<VaultKey>();
			var results = new VaultKeyCollection(database, database.GetQuery<VaultKeyColumns, VaultKey>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static VaultKeyCollection Where(WhereDelegate<VaultKeyColumns> where, OrderBy<VaultKeyColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<VaultKey>();
			var results = new VaultKeyCollection(database, database.GetQuery<VaultKeyColumns, VaultKey>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`VaultKeyColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static VaultKeyCollection Where(QiQuery where, Database database = null)
		{
			var results = new VaultKeyCollection(database, Select<VaultKeyColumns>.From<VaultKey>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static VaultKey GetOneWhere(QueryFilter where, Database database = null)
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
		public static VaultKey OneWhere(QueryFilter where, Database database = null)
		{
			WhereDelegate<VaultKeyColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<VaultKeyColumns> where, Database database = null)
		{
			SetOneWhere(where, out VaultKey ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<VaultKeyColumns> where, out VaultKey result, Database database = null)
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
		public static VaultKey GetOneWhere(WhereDelegate<VaultKeyColumns> where, Database database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				VaultKeyColumns c = new VaultKeyColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single VaultKey instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static VaultKey OneWhere(WhereDelegate<VaultKeyColumns> where, Database database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`VaultKeyColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static VaultKey OneWhere(QiQuery where, Database database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static VaultKey FirstOneWhere(WhereDelegate<VaultKeyColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static VaultKey FirstOneWhere(WhereDelegate<VaultKeyColumns> where, OrderBy<VaultKeyColumns> orderBy, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static VaultKey FirstOneWhere(QueryFilter where, OrderBy<VaultKeyColumns> orderBy = null, Database database = null)
		{
			WhereDelegate<VaultKeyColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static VaultKeyCollection Top(int count, WhereDelegate<VaultKeyColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static VaultKeyCollection Top(int count, WhereDelegate<VaultKeyColumns> where, OrderBy<VaultKeyColumns> orderBy, Database database = null)
		{
			VaultKeyColumns c = new VaultKeyColumns();
			IQueryFilter filter = where(c);

			Database db = database ?? Db.For<VaultKey>();
			QuerySet query = GetQuerySet(db);
			query.Top<VaultKey>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<VaultKeyColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<VaultKeyCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static VaultKeyCollection Top(int count, QueryFilter where, Database database)
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
		public static VaultKeyCollection Top(int count, QueryFilter where, OrderBy<VaultKeyColumns> orderBy = null, Database database = null)
		{
			Database db = database ?? Db.For<VaultKey>();
			QuerySet query = GetQuerySet(db);
			query.Top<VaultKey>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<VaultKeyColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<VaultKeyCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static VaultKeyCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, Database database = null)
		{
			Database db = database ?? Db.For<VaultKey>();
			QuerySet query = GetQuerySet(db);
			query.Top<VaultKey>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<VaultKeyCollection>(0);
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
		public static VaultKeyCollection Top(int count, QiQuery where, Database database = null)
		{
			Database db = database ?? Db.For<VaultKey>();
			QuerySet query = GetQuerySet(db);
			query.Top<VaultKey>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<VaultKeyCollection>(0);
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
			Database db = database ?? Db.For<VaultKey>();
            QuerySet query = GetQuerySet(db);
            query.Count<VaultKey>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a VaultKeyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between VaultKeyColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static long Count(WhereDelegate<VaultKeyColumns> where, Database database = null)
		{
			VaultKeyColumns c = new VaultKeyColumns();
			IQueryFilter filter = where(c) ;

			Database db = database ?? Db.For<VaultKey>();
			QuerySet query = GetQuerySet(db);
			query.Count<VaultKey>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, Database database = null)
		{
		    Database db = database ?? Db.For<VaultKey>();
			QuerySet query = GetQuerySet(db);
			query.Count<VaultKey>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static VaultKey CreateFromFilter(IQueryFilter filter, Database database = null)
		{
			Database db = database ?? Db.For<VaultKey>();
			var dao = new VaultKey();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static VaultKey OneOrThrow(VaultKeyCollection c)
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
