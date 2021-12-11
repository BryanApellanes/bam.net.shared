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
using Bam.Net.Server.ServiceProxy.Data;

namespace Bam.Net.Server.ServiceProxy.Data.Dao.Repository
{
	[Serializable]
	public class ServiceProxyServerRepository: DaoRepository
	{
		public ServiceProxyServerRepository()
		{
			SchemaName = "ServiceProxyServer";
			BaseNamespace = "Bam.Net.Server.ServiceProxy.Data";			

			
			AddType<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>();
			

			DaoAssembly = typeof(ServiceProxyServerRepository).Assembly;
		}

		object _addLock = new object();
        public override void AddType(Type type)
        {
            lock (_addLock)
            {
                base.AddType(type);
                DaoAssembly = typeof(ServiceProxyServerRepository).Assembly;
            }
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneSecureChannelSessionWhere(WhereDelegate<SecureChannelSessionColumns> where)
		{
			Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneSecureChannelSessionWhere(WhereDelegate<SecureChannelSessionColumns> where, out Bam.Net.Server.ServiceProxy.Data.SecureChannelSession result)
		{
			Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.SetOneWhere(where, out Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession daoResult, Database);
			result = daoResult.CopyAs<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>();
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Net.Server.ServiceProxy.Data.SecureChannelSession GetOneSecureChannelSessionWhere(WhereDelegate<SecureChannelSessionColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>();
			return (Bam.Net.Server.ServiceProxy.Data.SecureChannelSession)Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.GetOneWhere(where, Database)?.CopyAs(wrapperType, this);
		}

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single SecureChannelSession instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a SecureChannelSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		public Bam.Net.Server.ServiceProxy.Data.SecureChannelSession OneSecureChannelSessionWhere(WhereDelegate<SecureChannelSessionColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>();
            return (Bam.Net.Server.ServiceProxy.Data.SecureChannelSession)Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.OneWhere(where, Database)?.CopyAs(wrapperType, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Net.Server.ServiceProxy.Data.SecureChannelSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Net.Server.ServiceProxy.Data.SecureChannelSessionColumns and other values
		/// </param>
		public IEnumerable<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession> SecureChannelSessionsWhere(WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy = null)
        {
            return Wrap<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>(Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a SecureChannelSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
		public IEnumerable<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession> TopSecureChannelSessionsWhere(int count, WhereDelegate<SecureChannelSessionColumns> where)
        {
            return Wrap<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>(Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.Top(count, where, Database));
        }

        public IEnumerable<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession> TopSecureChannelSessionsWhere(int count, WhereDelegate<SecureChannelSessionColumns> where, OrderBy<SecureChannelSessionColumns> orderBy)
        {
            return Wrap<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>(Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of SecureChannelSessions
		/// </summary>
		public long CountSecureChannelSessions()
        {
            return Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a SecureChannelSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between SecureChannelSessionColumns and other values
		/// </param>
        public long CountSecureChannelSessionsWhere(WhereDelegate<SecureChannelSessionColumns> where)
        {
            return Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.Count(where, Database);
        }
        
        public async Task BatchQuerySecureChannelSessions(int batchSize, WhereDelegate<SecureChannelSessionColumns> where, Action<IEnumerable<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>> batchProcessor)
        {
            await Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>(batch));
            }, Database);
        }
		
        public async Task BatchAllSecureChannelSessions(int batchSize, Action<IEnumerable<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>> batchProcessor)
        {
            await Bam.Net.Server.ServiceProxy.Data.Dao.SecureChannelSession.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Net.Server.ServiceProxy.Data.SecureChannelSession>(batch));
            }, Database);
        }


	}
}																								
