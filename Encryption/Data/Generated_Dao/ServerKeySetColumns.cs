using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class ServerKeySetColumns: QueryFilter<ServerKeySetColumns>, IFilterToken
    {
        public ServerKeySetColumns() { }
        public ServerKeySetColumns(string columnName, bool isForeignKey = false)
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
        
		public ServerKeySetColumns KeyColumn => new ServerKeySetColumns("Id");

        public ServerKeySetColumns Id => new ServerKeySetColumns("Id");
        public ServerKeySetColumns Uuid => new ServerKeySetColumns("Uuid");
        public ServerKeySetColumns Cuid => new ServerKeySetColumns("Cuid");
        public ServerKeySetColumns ApplicationName => new ServerKeySetColumns("ApplicationName");
        public ServerKeySetColumns MachineName => new ServerKeySetColumns("MachineName");
        public ServerKeySetColumns ServerHostName => new ServerKeySetColumns("ServerHostName");
        public ServerKeySetColumns ClientHostName => new ServerKeySetColumns("ClientHostName");
        public ServerKeySetColumns Identifier => new ServerKeySetColumns("Identifier");
        public ServerKeySetColumns RsaKey => new ServerKeySetColumns("RsaKey");
        public ServerKeySetColumns AesKey => new ServerKeySetColumns("AesKey");
        public ServerKeySetColumns AesIV => new ServerKeySetColumns("AesIV");
        public ServerKeySetColumns Secret => new ServerKeySetColumns("Secret");
        public ServerKeySetColumns Key => new ServerKeySetColumns("Key");
        public ServerKeySetColumns CompositeKeyId => new ServerKeySetColumns("CompositeKeyId");
        public ServerKeySetColumns CompositeKey => new ServerKeySetColumns("CompositeKey");
        public ServerKeySetColumns CreatedBy => new ServerKeySetColumns("CreatedBy");
        public ServerKeySetColumns ModifiedBy => new ServerKeySetColumns("ModifiedBy");
        public ServerKeySetColumns Modified => new ServerKeySetColumns("Modified");
        public ServerKeySetColumns Deleted => new ServerKeySetColumns("Deleted");
        public ServerKeySetColumns Created => new ServerKeySetColumns("Created");


		public Type DaoType => typeof(ServerKeySet);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}