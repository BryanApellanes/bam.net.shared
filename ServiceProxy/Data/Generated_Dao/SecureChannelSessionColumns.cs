using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam.Net.Data;

namespace Bam.Net.ServiceProxy.Data.Dao
{
    public class SecureChannelSessionColumns: QueryFilter<SecureChannelSessionColumns>, IFilterToken
    {
        public SecureChannelSessionColumns() { }
        public SecureChannelSessionColumns(string columnName, bool isForeignKey = false)
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
        
		public SecureChannelSessionColumns KeyColumn => new SecureChannelSessionColumns("Id");

        public SecureChannelSessionColumns Id => new SecureChannelSessionColumns("Id");
        public SecureChannelSessionColumns Uuid => new SecureChannelSessionColumns("Uuid");
        public SecureChannelSessionColumns Cuid => new SecureChannelSessionColumns("Cuid");
        public SecureChannelSessionColumns Identifier => new SecureChannelSessionColumns("Identifier");
        public SecureChannelSessionColumns AsymmetricKey => new SecureChannelSessionColumns("AsymmetricKey");
        public SecureChannelSessionColumns SymmetricKey => new SecureChannelSessionColumns("SymmetricKey");
        public SecureChannelSessionColumns SymmetricIV => new SecureChannelSessionColumns("SymmetricIV");
        public SecureChannelSessionColumns TimeOffset => new SecureChannelSessionColumns("TimeOffset");
        public SecureChannelSessionColumns LastActivity => new SecureChannelSessionColumns("LastActivity");
        public SecureChannelSessionColumns Expires => new SecureChannelSessionColumns("Expires");
        public SecureChannelSessionColumns Server => new SecureChannelSessionColumns("Server");
        public SecureChannelSessionColumns Client => new SecureChannelSessionColumns("Client");
        public SecureChannelSessionColumns Key => new SecureChannelSessionColumns("Key");
        public SecureChannelSessionColumns CompositeKeyId => new SecureChannelSessionColumns("CompositeKeyId");
        public SecureChannelSessionColumns CompositeKey => new SecureChannelSessionColumns("CompositeKey");
        public SecureChannelSessionColumns CreatedBy => new SecureChannelSessionColumns("CreatedBy");
        public SecureChannelSessionColumns ModifiedBy => new SecureChannelSessionColumns("ModifiedBy");
        public SecureChannelSessionColumns Modified => new SecureChannelSessionColumns("Modified");
        public SecureChannelSessionColumns Deleted => new SecureChannelSessionColumns("Deleted");
        public SecureChannelSessionColumns Created => new SecureChannelSessionColumns("Created");


		public Type DaoType => typeof(SecureChannelSession);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}