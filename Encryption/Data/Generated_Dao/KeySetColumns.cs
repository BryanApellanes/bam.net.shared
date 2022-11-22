using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class KeySetColumns: QueryFilter<KeySetColumns>, IFilterToken
    {
        public KeySetColumns() { }
        public KeySetColumns(string columnName, bool isForeignKey = false)
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
        
		public KeySetColumns KeyColumn => new KeySetColumns("Id");

        public KeySetColumns Id => new KeySetColumns("Id");
        public KeySetColumns Uuid => new KeySetColumns("Uuid");
        public KeySetColumns Cuid => new KeySetColumns("Cuid");
        public KeySetColumns Identifier => new KeySetColumns("Identifier");
        public KeySetColumns RsaKey => new KeySetColumns("RsaKey");
        public KeySetColumns AesKey => new KeySetColumns("AesKey");
        public KeySetColumns AesIV => new KeySetColumns("AesIV");
        public KeySetColumns Secret => new KeySetColumns("Secret");
        public KeySetColumns Key => new KeySetColumns("Key");
        public KeySetColumns CompositeKeyId => new KeySetColumns("CompositeKeyId");
        public KeySetColumns CompositeKey => new KeySetColumns("CompositeKey");
        public KeySetColumns CreatedBy => new KeySetColumns("CreatedBy");
        public KeySetColumns ModifiedBy => new KeySetColumns("ModifiedBy");
        public KeySetColumns Modified => new KeySetColumns("Modified");
        public KeySetColumns Deleted => new KeySetColumns("Deleted");
        public KeySetColumns Created => new KeySetColumns("Created");


		public Type DaoType => typeof(KeySet);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}