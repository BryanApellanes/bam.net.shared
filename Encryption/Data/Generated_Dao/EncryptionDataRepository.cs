/*
This file was generated and should not be modified directly
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Bam.Net;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Bam.Net.Encryption.Data;

namespace Bam.Net.Encryption.Data.Dao.Repository
{
	[Serializable]
	public class EncryptionDataRepository: DaoInheritanceRepository
	{
		public EncryptionDataRepository()
		{
			SchemaName = "EncryptionData";
			BaseNamespace = "Bam.Net.Encryption.Data";			

			
			AddType<Bam.Net.Encryption.Data.ClientKeySet>();
			
			
			AddType<Bam.Net.Encryption.Data.KeySet>();
			
			
			AddType<Bam.Net.Encryption.Data.ServerKeySet>();
			

			DaoAssembly = typeof(EncryptionDataRepository).Assembly;
		}

		object _addLock = new object();
        public override void AddType(Type type)
        {
            lock (_addLock)
            {
                base.AddType(type);
                DaoAssembly = typeof(EncryptionDataRepository).Assembly;
            }
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneClientKeySetWhere(WhereDelegate<ClientKeySetColumns> where)
		{
			Bam.Net.Encryption.Data.Dao.ClientKeySet.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneClientKeySetWhere(WhereDelegate<ClientKeySetColumns> where, out Bam.Net.Encryption.Data.ClientKeySet result)
		{
			Bam.Net.Encryption.Data.Dao.ClientKeySet.SetOneWhere(where, out Bam.Net.Encryption.Data.Dao.ClientKeySet daoResult, Database);
			result = daoResult.CopyAs<Bam.Net.Encryption.Data.ClientKeySet>();
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Net.Encryption.Data.ClientKeySet GetOneClientKeySetWhere(WhereDelegate<ClientKeySetColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Net.Encryption.Data.ClientKeySet>();
			return (Bam.Net.Encryption.Data.ClientKeySet)Bam.Net.Encryption.Data.Dao.ClientKeySet.GetOneWhere(where, Database)?.CopyAs(wrapperType, this);
		}

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ClientKeySet instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ClientKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		public Bam.Net.Encryption.Data.ClientKeySet OneClientKeySetWhere(WhereDelegate<ClientKeySetColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Net.Encryption.Data.ClientKeySet>();
            return (Bam.Net.Encryption.Data.ClientKeySet)Bam.Net.Encryption.Data.Dao.ClientKeySet.OneWhere(where, Database)?.CopyAs(wrapperType, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Net.Encryption.Data.ClientKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Net.Encryption.Data.ClientKeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Net.Encryption.Data.ClientKeySet> ClientKeySetsWhere(WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy = null)
        {
            return Wrap<Bam.Net.Encryption.Data.ClientKeySet>(Bam.Net.Encryption.Data.Dao.ClientKeySet.Where(where, orderBy, Database));
        }
		
		/// <summary>
		/// Execute a query and return the specified number
		/// of values. This method issues a sql TOP clause so only the 
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this 
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that receives a ClientKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Net.Encryption.Data.ClientKeySet> TopClientKeySetsWhere(int count, WhereDelegate<ClientKeySetColumns> where)
        {
            return Wrap<Bam.Net.Encryption.Data.ClientKeySet>(Bam.Net.Encryption.Data.Dao.ClientKeySet.Top(count, where, Database));
        }

        public IEnumerable<Bam.Net.Encryption.Data.ClientKeySet> TopClientKeySetsWhere(int count, WhereDelegate<ClientKeySetColumns> where, OrderBy<ClientKeySetColumns> orderBy)
        {
            return Wrap<Bam.Net.Encryption.Data.ClientKeySet>(Bam.Net.Encryption.Data.Dao.ClientKeySet.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ClientKeySets
		/// </summary>
		public long CountClientKeySets()
        {
            return Bam.Net.Encryption.Data.Dao.ClientKeySet.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ClientKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetColumns and other values
		/// </param>
        public long CountClientKeySetsWhere(WhereDelegate<ClientKeySetColumns> where)
        {
            return Bam.Net.Encryption.Data.Dao.ClientKeySet.Count(where, Database);
        }
        
        public async Task BatchQueryClientKeySets(int batchSize, WhereDelegate<ClientKeySetColumns> where, Action<IEnumerable<Bam.Net.Encryption.Data.ClientKeySet>> batchProcessor)
        {
            await Bam.Net.Encryption.Data.Dao.ClientKeySet.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Net.Encryption.Data.ClientKeySet>(batch));
            }, Database);
        }
		
        public async Task BatchAllClientKeySets(int batchSize, Action<IEnumerable<Bam.Net.Encryption.Data.ClientKeySet>> batchProcessor)
        {
            await Bam.Net.Encryption.Data.Dao.ClientKeySet.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Net.Encryption.Data.ClientKeySet>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneKeySetWhere(WhereDelegate<KeySetColumns> where)
		{
			Bam.Net.Encryption.Data.Dao.KeySet.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneKeySetWhere(WhereDelegate<KeySetColumns> where, out Bam.Net.Encryption.Data.KeySet result)
		{
			Bam.Net.Encryption.Data.Dao.KeySet.SetOneWhere(where, out Bam.Net.Encryption.Data.Dao.KeySet daoResult, Database);
			result = daoResult.CopyAs<Bam.Net.Encryption.Data.KeySet>();
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Net.Encryption.Data.KeySet GetOneKeySetWhere(WhereDelegate<KeySetColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Net.Encryption.Data.KeySet>();
			return (Bam.Net.Encryption.Data.KeySet)Bam.Net.Encryption.Data.Dao.KeySet.GetOneWhere(where, Database)?.CopyAs(wrapperType, this);
		}

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single KeySet instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a KeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between KeySetColumns and other values
		/// </param>
		public Bam.Net.Encryption.Data.KeySet OneKeySetWhere(WhereDelegate<KeySetColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Net.Encryption.Data.KeySet>();
            return (Bam.Net.Encryption.Data.KeySet)Bam.Net.Encryption.Data.Dao.KeySet.OneWhere(where, Database)?.CopyAs(wrapperType, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Net.Encryption.Data.KeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Net.Encryption.Data.KeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Net.Encryption.Data.KeySet> KeySetsWhere(WhereDelegate<KeySetColumns> where, OrderBy<KeySetColumns> orderBy = null)
        {
            return Wrap<Bam.Net.Encryption.Data.KeySet>(Bam.Net.Encryption.Data.Dao.KeySet.Where(where, orderBy, Database));
        }
		
		/// <summary>
		/// Execute a query and return the specified number
		/// of values. This method issues a sql TOP clause so only the 
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this 
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that receives a KeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between KeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Net.Encryption.Data.KeySet> TopKeySetsWhere(int count, WhereDelegate<KeySetColumns> where)
        {
            return Wrap<Bam.Net.Encryption.Data.KeySet>(Bam.Net.Encryption.Data.Dao.KeySet.Top(count, where, Database));
        }

        public IEnumerable<Bam.Net.Encryption.Data.KeySet> TopKeySetsWhere(int count, WhereDelegate<KeySetColumns> where, OrderBy<KeySetColumns> orderBy)
        {
            return Wrap<Bam.Net.Encryption.Data.KeySet>(Bam.Net.Encryption.Data.Dao.KeySet.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of KeySets
		/// </summary>
		public long CountKeySets()
        {
            return Bam.Net.Encryption.Data.Dao.KeySet.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a KeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between KeySetColumns and other values
		/// </param>
        public long CountKeySetsWhere(WhereDelegate<KeySetColumns> where)
        {
            return Bam.Net.Encryption.Data.Dao.KeySet.Count(where, Database);
        }
        
        public async Task BatchQueryKeySets(int batchSize, WhereDelegate<KeySetColumns> where, Action<IEnumerable<Bam.Net.Encryption.Data.KeySet>> batchProcessor)
        {
            await Bam.Net.Encryption.Data.Dao.KeySet.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Net.Encryption.Data.KeySet>(batch));
            }, Database);
        }
		
        public async Task BatchAllKeySets(int batchSize, Action<IEnumerable<Bam.Net.Encryption.Data.KeySet>> batchProcessor)
        {
            await Bam.Net.Encryption.Data.Dao.KeySet.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Net.Encryption.Data.KeySet>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerKeySetWhere(WhereDelegate<ServerKeySetColumns> where)
		{
			Bam.Net.Encryption.Data.Dao.ServerKeySet.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerKeySetWhere(WhereDelegate<ServerKeySetColumns> where, out Bam.Net.Encryption.Data.ServerKeySet result)
		{
			Bam.Net.Encryption.Data.Dao.ServerKeySet.SetOneWhere(where, out Bam.Net.Encryption.Data.Dao.ServerKeySet daoResult, Database);
			result = daoResult.CopyAs<Bam.Net.Encryption.Data.ServerKeySet>();
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Net.Encryption.Data.ServerKeySet GetOneServerKeySetWhere(WhereDelegate<ServerKeySetColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Net.Encryption.Data.ServerKeySet>();
			return (Bam.Net.Encryption.Data.ServerKeySet)Bam.Net.Encryption.Data.Dao.ServerKeySet.GetOneWhere(where, Database)?.CopyAs(wrapperType, this);
		}

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ServerKeySet instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerKeySetColumns and other values
		/// </param>
		public Bam.Net.Encryption.Data.ServerKeySet OneServerKeySetWhere(WhereDelegate<ServerKeySetColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Net.Encryption.Data.ServerKeySet>();
            return (Bam.Net.Encryption.Data.ServerKeySet)Bam.Net.Encryption.Data.Dao.ServerKeySet.OneWhere(where, Database)?.CopyAs(wrapperType, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Net.Encryption.Data.ServerKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Net.Encryption.Data.ServerKeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Net.Encryption.Data.ServerKeySet> ServerKeySetsWhere(WhereDelegate<ServerKeySetColumns> where, OrderBy<ServerKeySetColumns> orderBy = null)
        {
            return Wrap<Bam.Net.Encryption.Data.ServerKeySet>(Bam.Net.Encryption.Data.Dao.ServerKeySet.Where(where, orderBy, Database));
        }
		
		/// <summary>
		/// Execute a query and return the specified number
		/// of values. This method issues a sql TOP clause so only the 
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this 
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that receives a ServerKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerKeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Net.Encryption.Data.ServerKeySet> TopServerKeySetsWhere(int count, WhereDelegate<ServerKeySetColumns> where)
        {
            return Wrap<Bam.Net.Encryption.Data.ServerKeySet>(Bam.Net.Encryption.Data.Dao.ServerKeySet.Top(count, where, Database));
        }

        public IEnumerable<Bam.Net.Encryption.Data.ServerKeySet> TopServerKeySetsWhere(int count, WhereDelegate<ServerKeySetColumns> where, OrderBy<ServerKeySetColumns> orderBy)
        {
            return Wrap<Bam.Net.Encryption.Data.ServerKeySet>(Bam.Net.Encryption.Data.Dao.ServerKeySet.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ServerKeySets
		/// </summary>
		public long CountServerKeySets()
        {
            return Bam.Net.Encryption.Data.Dao.ServerKeySet.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerKeySetColumns and other values
		/// </param>
        public long CountServerKeySetsWhere(WhereDelegate<ServerKeySetColumns> where)
        {
            return Bam.Net.Encryption.Data.Dao.ServerKeySet.Count(where, Database);
        }
        
        public async Task BatchQueryServerKeySets(int batchSize, WhereDelegate<ServerKeySetColumns> where, Action<IEnumerable<Bam.Net.Encryption.Data.ServerKeySet>> batchProcessor)
        {
            await Bam.Net.Encryption.Data.Dao.ServerKeySet.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Net.Encryption.Data.ServerKeySet>(batch));
            }, Database);
        }
		
        public async Task BatchAllServerKeySets(int batchSize, Action<IEnumerable<Bam.Net.Encryption.Data.ServerKeySet>> batchProcessor)
        {
            await Bam.Net.Encryption.Data.Dao.ServerKeySet.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Net.Encryption.Data.ServerKeySet>(batch));
            }, Database);
        }


	}
}																								
