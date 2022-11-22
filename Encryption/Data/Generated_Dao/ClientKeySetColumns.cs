using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class ClientKeySetColumns: QueryFilter<ClientKeySetColumns>, IFilterToken
    {
        public ClientKeySetColumns() { }
        public ClientKeySetColumns(string columnName, bool isForeignKey = false)
            : base(columnName)
        { 
            _isForeignKey = isForeignKey;
        }
        
        public bool IsKey()
        {
            return (bool)ColumnName?.Equals(KeyColumn.ColumnName);
        }

        private bool? _isForeignKey;
        public bool IsForeignKey
        {
            get
            {
                if (_isForeignKey == null)
                {
                    PropertyInfo prop = DaoType
                        .GetProperties()
                        .FirstOrDefault(pi => ((MemberInfo) pi)
                            .HasCustomAttributeOfType<ForeignKeyAttribute>(out ForeignKeyAttribute foreignKeyAttribute)
                                && foreignKeyAttribute.Name.Equals(ColumnName));
                        _isForeignKey = prop != null;
                }

                return _isForeignKey.Value;
            }
            set => _isForeignKey = value;
        }
        
		public ClientKeySetColumns KeyColumn => new ClientKeySetColumns("Id");

        public ClientKeySetColumns Id => new ClientKeySetColumns("Id");
        public ClientKeySetColumns Uuid => new ClientKeySetColumns("Uuid");
        public ClientKeySetColumns Cuid => new ClientKeySetColumns("Cuid");
        public ClientKeySetColumns MachineName => new ClientKeySetColumns("MachineName");
        public ClientKeySetColumns ClientHostName => new ClientKeySetColumns("ClientHostName");
        public ClientKeySetColumns ServerHostName => new ClientKeySetColumns("ServerHostName");
        public ClientKeySetColumns PublicKey => new ClientKeySetColumns("PublicKey");
        public ClientKeySetColumns Identifier => new ClientKeySetColumns("Identifier");
        public ClientKeySetColumns AesKey => new ClientKeySetColumns("AesKey");
        public ClientKeySetColumns AesIV => new ClientKeySetColumns("AesIV");
        public ClientKeySetColumns ApplicationName => new ClientKeySetColumns("ApplicationName");
        public ClientKeySetColumns Secret => new ClientKeySetColumns("Secret");
        public ClientKeySetColumns Key => new ClientKeySetColumns("Key");
        public ClientKeySetColumns CompositeKeyId => new ClientKeySetColumns("CompositeKeyId");
        public ClientKeySetColumns CompositeKey => new ClientKeySetColumns("CompositeKey");
        public ClientKeySetColumns CreatedBy => new ClientKeySetColumns("CreatedBy");
        public ClientKeySetColumns ModifiedBy => new ClientKeySetColumns("ModifiedBy");
        public ClientKeySetColumns Modified => new ClientKeySetColumns("Modified");
        public ClientKeySetColumns Deleted => new ClientKeySetColumns("Deleted");
        public ClientKeySetColumns Created => new ClientKeySetColumns("Created");


		public Type DaoType => typeof(ClientKeySet);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}