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

namespace Bam.Net.CoreServices.ApplicationRegistration.Data.Dao
{
	// schema = ApplicationRegistration
	// connection Name = ApplicationRegistration
	[Serializable]
	[Bam.Net.Data.Table("Application", "ApplicationRegistration")]
	public partial class Application: Bam.Net.Data.Dao
	{
		public Application():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public Application(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public Application(Database db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public Application(Database db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Net.Exclude]
		public static implicit operator Application(DataRow data)
		{
			return new Application(data);
		}

		private void SetChildren()
		{


			if(_database != null)
			{
				this.ChildCollections.Add("ApiKey_ApplicationId", new ApiKeyCollection(Database.GetQuery<ApiKeyColumns, ApiKey>((c) => c.ApplicationId == GetULongValue("Id")), this, "ApplicationId"));
			}
			if(_database != null)
			{
				this.ChildCollections.Add("ProcessDescriptor_ApplicationId", new ProcessDescriptorCollection(Database.GetQuery<ProcessDescriptorColumns, ProcessDescriptor>((c) => c.ApplicationId == GetULongValue("Id")), this, "ApplicationId"));
			}
			if(_database != null)
			{
				this.ChildCollections.Add("Configuration_ApplicationId", new ConfigurationCollection(Database.GetQuery<ConfigurationColumns, Configuration>((c) => c.ApplicationId == GetULongValue("Id")), this, "ApplicationId"));
			}
			if(_database != null)
			{
				this.ChildCollections.Add("Client_ApplicationId", new ClientCollection(Database.GetQuery<ClientColumns, Client>((c) => c.ApplicationId == GetULongValue("Id")), this, "ApplicationId"));
			}
			if(_database != null)
			{
				this.ChildCollections.Add("ApplicationHostDomain_ApplicationId", new ApplicationHostDomainCollection(Database.GetQuery<ApplicationHostDomainColumns, ApplicationHostDomain>((c) => c.ApplicationId == GetULongValue("Id")), this, "ApplicationId"));
			}
			if(_database != null)
			{
				this.ChildCollections.Add("ApplicationMachine_ApplicationId", new ApplicationMachineCollection(Database.GetQuery<ApplicationMachineColumns, ApplicationMachine>((c) => c.ApplicationId == GetULongValue("Id")), this, "ApplicationId"));
			}
            this.ChildCollections.Add("Application_ApplicationHostDomain_HostDomain",  new XrefDaoCollection<ApplicationHostDomain, HostDomain>(this, false));
            this.ChildCollections.Add("Application_ApplicationMachine_Machine",  new XrefDaoCollection<ApplicationMachine, Machine>(this, false));


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

	// property:Name, columnName: Name	
	[Bam.Net.Data.Column(Name="Name", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string Name
	{
		get
		{
			return GetStringValue("Name");
		}
		set
		{
			SetValue("Name", value);
		}
	}

	// property:Description, columnName: Description	
	[Bam.Net.Data.Column(Name="Description", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
	public string Description
	{
		get
		{
			return GetStringValue("Description");
		}
		set
		{
			SetValue("Description", value);
		}
	}

	// property:OrganizationKey, columnName: OrganizationKey	
	[Bam.Net.Data.Column(Name="OrganizationKey", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
	public ulong? OrganizationKey
	{
		get
		{
			return GetULongValue("OrganizationKey");
		}
		set
		{
			SetValue("OrganizationKey", value);
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


	// start OrganizationId -> OrganizationId
	[Bam.Net.Data.ForeignKey(
        Table="Application",
		Name="OrganizationId",
		DbDataType="BigInt",
		MaxLength="",
		AllowNull=true,
		ReferencedKey="Id",
		ReferencedTable="Organization",
		Suffix="1")]
	public ulong? OrganizationId
	{
		get
		{
			return GetULongValue("OrganizationId");
		}
		set
		{
			SetValue("OrganizationId", value);
		}
	}

    Organization _organizationOfOrganizationId;
	public Organization OrganizationOfOrganizationId
	{
		get
		{
			if(_organizationOfOrganizationId == null)
			{
				_organizationOfOrganizationId = Bam.Net.CoreServices.ApplicationRegistration.Data.Dao.Organization.OneWhere(c => c.KeyColumn == this.OrganizationId, this.Database);
			}
			return _organizationOfOrganizationId;
		}
	}


	[Bam.Net.Exclude]	
	public ApiKeyCollection ApiKeysByApplicationId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
			}

			if(!this.ChildCollections.ContainsKey("ApiKey_ApplicationId"))
			{
				SetChildren();
			}

			var c = (ApiKeyCollection)this.ChildCollections["ApiKey_ApplicationId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}
		[Bam.Net.Exclude]	
	public ProcessDescriptorCollection ProcessDescriptorsByApplicationId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
			}

			if(!this.ChildCollections.ContainsKey("ProcessDescriptor_ApplicationId"))
			{
				SetChildren();
			}

			var c = (ProcessDescriptorCollection)this.ChildCollections["ProcessDescriptor_ApplicationId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}
		[Bam.Net.Exclude]	
	public ConfigurationCollection ConfigurationsByApplicationId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
			}

			if(!this.ChildCollections.ContainsKey("Configuration_ApplicationId"))
			{
				SetChildren();
			}

			var c = (ConfigurationCollection)this.ChildCollections["Configuration_ApplicationId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}
		[Bam.Net.Exclude]	
	public ClientCollection ClientsByApplicationId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
			}

			if(!this.ChildCollections.ContainsKey("Client_ApplicationId"))
			{
				SetChildren();
			}

			var c = (ClientCollection)this.ChildCollections["Client_ApplicationId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}
		[Bam.Net.Exclude]	
	public ApplicationHostDomainCollection ApplicationHostDomainsByApplicationId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
			}

			if(!this.ChildCollections.ContainsKey("ApplicationHostDomain_ApplicationId"))
			{
				SetChildren();
			}

			var c = (ApplicationHostDomainCollection)this.ChildCollections["ApplicationHostDomain_ApplicationId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}
		[Bam.Net.Exclude]	
	public ApplicationMachineCollection ApplicationMachinesByApplicationId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
			}

			if(!this.ChildCollections.ContainsKey("ApplicationMachine_ApplicationId"))
			{
				SetChildren();
			}

			var c = (ApplicationMachineCollection)this.ChildCollections["ApplicationMachine_ApplicationId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}
	
		// Xref       
        public XrefDaoCollection<ApplicationHostDomain, HostDomain> HostDomains
        {
            get
            {			
				if (this.IsNew)
				{
					throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
				}

				if(!this.ChildCollections.ContainsKey("Application_ApplicationHostDomain_HostDomain"))
				{
					SetChildren();
				}

				var xref = (XrefDaoCollection<ApplicationHostDomain, HostDomain>)this.ChildCollections["Application_ApplicationHostDomain_HostDomain"];
				if(!xref.Loaded)
				{
					xref.Load(Database);
				}

				return xref;
            }
        }		// Xref       
        public XrefDaoCollection<ApplicationMachine, Machine> Machines
        {
            get
            {			
				if (this.IsNew)
				{
					throw new InvalidOperationException("The current instance of type({0}) hasn't been saved and will have no child collections, call Save() or Save(Database) first."._Format(this.GetType().Name));
				}

				if(!this.ChildCollections.ContainsKey("Application_ApplicationMachine_Machine"))
				{
					SetChildren();
				}

				var xref = (XrefDaoCollection<ApplicationMachine, Machine>)this.ChildCollections["Application_ApplicationMachine_Machine"];
				if(!xref.Loaded)
				{
					xref.Load(Database);
				}

				return xref;
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
				var colFilter = new ApplicationColumns();
				return (colFilter.KeyColumn == IdValue);
			}
		}

		/// <summary>
        /// Return every record in the Application table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static ApplicationCollection LoadAll(Database database = null)
		{
			Database db = database ?? Db.For<Application>();
            SqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<Application>();
            var results = new ApplicationCollection(db, sql.GetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Net.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<Application>> batchProcessor, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				ApplicationColumns columns = new ApplicationColumns();
				var orderBy = Bam.Net.Data.Order.By<ApplicationColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
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
		public static async Task BatchQuery(int batchSize, QueryFilter filter, Action<IEnumerable<Application>> batchProcessor, Database database = null)
		{
			await BatchQuery(batchSize, (c) => filter, batchProcessor, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery(int batchSize, WhereDelegate<ApplicationColumns> where, Action<IEnumerable<Application>> batchProcessor, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				ApplicationColumns columns = new ApplicationColumns();
				var orderBy = Bam.Net.Data.Order.By<ApplicationColumns>(c => c.KeyColumn, Bam.Net.Data.SortOrder.Ascending);
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (ApplicationColumns)where(columns) && columns.KeyColumn > topId, orderBy, database);
				}
			});
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, QueryFilter filter, Action<IEnumerable<Application>> batchProcessor, Bam.Net.Data.OrderBy<ApplicationColumns> orderBy, Database database = null)
		{
			await BatchQuery<ColType>(batchSize, (c) => filter, batchProcessor, orderBy, database);
		}

		/// <summary>
		/// Process results of a query in batches of the specified size
		/// </summary>
		[Bam.Net.Exclude]
		public static async Task BatchQuery<ColType>(int batchSize, WhereDelegate<ApplicationColumns> where, Action<IEnumerable<Application>> batchProcessor, Bam.Net.Data.OrderBy<ApplicationColumns> orderBy, Database database = null)
		{
			await System.Threading.Tasks.Task.Run(async ()=>
			{
				ApplicationColumns columns = new ApplicationColumns();
				var results = Top(batchSize, where, orderBy, database);
				while(results.Count > 0)
				{
					await System.Threading.Tasks.Task.Run(()=>
					{
						batchProcessor(results);
					});
					ColType top = results.Select(d => d.Property<ColType>(orderBy.Column.ToString())).ToArray().Largest();
					results = Top(batchSize, (ApplicationColumns)where(columns) && orderBy.Column > top, orderBy, database);
				}
			});
		}

		public static Application GetById(uint? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified Application.Id was null");
			return GetById(id.Value, database);
		}

		public static Application GetById(uint id, Database database = null)
		{
			return GetById((ulong)id, database);
		}

		public static Application GetById(int? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified Application.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static Application GetById(int id, Database database = null)
		{
			return GetById((long)id, database);
		}

		public static Application GetById(long? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified Application.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static Application GetById(long id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static Application GetById(ulong? id, Database database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified Application.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static Application GetById(ulong id, Database database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static Application GetByUuid(string uuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Uuid") == uuid, database);
		}

		public static Application GetByCuid(string cuid, Database database = null)
		{
			return OneWhere(c => Bam.Net.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Net.Exclude]
		public static ApplicationCollection Query(QueryFilter filter, Database database = null)
		{
			return Where(filter, database);
		}

		[Bam.Net.Exclude]
		public static ApplicationCollection Where(QueryFilter filter, Database database = null)
		{
			WhereDelegate<ApplicationColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a ApplicationColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static ApplicationCollection Where(Func<ApplicationColumns, QueryFilter<ApplicationColumns>> where, OrderBy<ApplicationColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<Application>();
			return new ApplicationCollection(database.GetQuery<ApplicationColumns, Application>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Net.Exclude]
		public static ApplicationCollection Where(WhereDelegate<ApplicationColumns> where, Database database = null)
		{
			database = database ?? Db.For<Application>();
			var results = new ApplicationCollection(database, database.GetQuery<ApplicationColumns, Application>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static ApplicationCollection Where(WhereDelegate<ApplicationColumns> where, OrderBy<ApplicationColumns> orderBy = null, Database database = null)
		{
			database = database ?? Db.For<Application>();
			var results = new ApplicationCollection(database, database.GetQuery<ApplicationColumns, Application>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`ApplicationColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static ApplicationCollection Where(QiQuery where, Database database = null)
		{
			var results = new ApplicationCollection(database, Select<ApplicationColumns>.From<Application>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static Application GetOneWhere(QueryFilter where, Database database = null)
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
		public static Application OneWhere(QueryFilter where, Database database = null)
		{
			WhereDelegate<ApplicationColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<ApplicationColumns> where, Database database = null)
		{
			SetOneWhere(where, out Application ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Net.Exclude]
		public static void SetOneWhere(WhereDelegate<ApplicationColumns> where, out Application result, Database database = null)
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
		public static Application GetOneWhere(WhereDelegate<ApplicationColumns> where, Database database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				ApplicationColumns c = new ApplicationColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single Application instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static Application OneWhere(WhereDelegate<ApplicationColumns> where, Database database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`ApplicationColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static Application OneWhere(QiQuery where, Database database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static Application FirstOneWhere(WhereDelegate<ApplicationColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static Application FirstOneWhere(WhereDelegate<ApplicationColumns> where, OrderBy<ApplicationColumns> orderBy, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static Application FirstOneWhere(QueryFilter where, OrderBy<ApplicationColumns> orderBy = null, Database database = null)
		{
			WhereDelegate<ApplicationColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Net.Exclude]
		public static ApplicationCollection Top(int count, WhereDelegate<ApplicationColumns> where, Database database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static ApplicationCollection Top(int count, WhereDelegate<ApplicationColumns> where, OrderBy<ApplicationColumns> orderBy, Database database = null)
		{
			ApplicationColumns c = new ApplicationColumns();
			IQueryFilter filter = where(c);

			Database db = database ?? Db.For<Application>();
			QuerySet query = GetQuerySet(db);
			query.Top<Application>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<ApplicationColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<ApplicationCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static ApplicationCollection Top(int count, QueryFilter where, Database database)
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
		public static ApplicationCollection Top(int count, QueryFilter where, OrderBy<ApplicationColumns> orderBy = null, Database database = null)
		{
			Database db = database ?? Db.For<Application>();
			QuerySet query = GetQuerySet(db);
			query.Top<Application>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<ApplicationColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<ApplicationCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Net.Exclude]
		public static ApplicationCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, Database database = null)
		{
			Database db = database ?? Db.For<Application>();
			QuerySet query = GetQuerySet(db);
			query.Top<Application>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<ApplicationCollection>(0);
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
		public static ApplicationCollection Top(int count, QiQuery where, Database database = null)
		{
			Database db = database ?? Db.For<Application>();
			QuerySet query = GetQuerySet(db);
			query.Top<Application>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<ApplicationCollection>(0);
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
			Database db = database ?? Db.For<Application>();
            QuerySet query = GetQuerySet(db);
            query.Count<Application>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ApplicationColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ApplicationColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Net.Exclude]
		public static long Count(WhereDelegate<ApplicationColumns> where, Database database = null)
		{
			ApplicationColumns c = new ApplicationColumns();
			IQueryFilter filter = where(c) ;

			Database db = database ?? Db.For<Application>();
			QuerySet query = GetQuerySet(db);
			query.Count<Application>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, Database database = null)
		{
		    Database db = database ?? Db.For<Application>();
			QuerySet query = GetQuerySet(db);
			query.Count<Application>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static Application CreateFromFilter(IQueryFilter filter, Database database = null)
		{
			Database db = database ?? Db.For<Application>();
			var dao = new Application();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static Application OneOrThrow(ApplicationCollection c)
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
