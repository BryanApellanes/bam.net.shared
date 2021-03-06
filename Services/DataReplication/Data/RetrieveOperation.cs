/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Data.Repositories;
using Bam.Net.Services.DataReplication;

namespace Bam.Net.Services.DataReplication.Data
{
    [Serializable]
    public class RetrieveOperation: Operation
	{
        public UniversalIdentifiers UniversalIdentifier { get; set; }
        public string Identifier { get; set; }
		public override object Execute(IDistributedRepository repository)
		{
            return repository.Retrieve(this);
		}

		public static RetrieveOperation For(CompositeKeyAuditRepoData data)
		{
			return For(data.GetType(), data.CompositeKeyId.ToString(), UniversalIdentifiers.CKey);
		}
		
        public static RetrieveOperation For(Type type, string identifier, UniversalIdentifiers universalIdentifier = UniversalIdentifiers.Cuid)
        {
            RetrieveOperation operation = For<RetrieveOperation>(type);
            operation.UniversalIdentifier = universalIdentifier;
            operation.Identifier = identifier;
            return operation;
        }
	}
}