using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bam.Net.Data;

namespace Bam.Net.Encryption.Data.Dao
{
    public class ApplicationKeySetColumns: QueryFilter<ApplicationKeySetColumns>, IFilterToken
    {
        public ApplicationKeySetColumns() { }
        public ApplicationKeySetColumns(string columnName, bool isForeignKey = false)
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
        
		public ApplicationKeySetColumns KeyColumn => new ApplicationKeySetColumns("Id");

        public ApplicationKeySetColumns Id => new ApplicationKeySetColumns("Id");
        public ApplicationKeySetColumns Uuid => new ApplicationKeySetColumns("Uuid");
        public ApplicationKeySetColumns Cuid => new ApplicationKeySetColumns("Cuid");
        public ApplicationKeySetColumns ApplicationName => new ApplicationKeySetColumns("ApplicationName");
        public ApplicationKeySetColumns Identifier => new ApplicationKeySetColumns("Identifier");
        public ApplicationKeySetColumns RsaKey => new ApplicationKeySetColumns("RsaKey");
        public ApplicationKeySetColumns AesKey => new ApplicationKeySetColumns("AesKey");
        public ApplicationKeySetColumns AesIV => new ApplicationKeySetColumns("AesIV");
        public ApplicationKeySetColumns Secret => new ApplicationKeySetColumns("Secret");
        public ApplicationKeySetColumns Key => new ApplicationKeySetColumns("Key");
        public ApplicationKeySetColumns CompositeKeyId => new ApplicationKeySetColumns("CompositeKeyId");
        public ApplicationKeySetColumns CompositeKey => new ApplicationKeySetColumns("CompositeKey");
        public ApplicationKeySetColumns CreatedBy => new ApplicationKeySetColumns("CreatedBy");
        public ApplicationKeySetColumns ModifiedBy => new ApplicationKeySetColumns("ModifiedBy");
        public ApplicationKeySetColumns Modified => new ApplicationKeySetColumns("Modified");
        public ApplicationKeySetColumns Deleted => new ApplicationKeySetColumns("Deleted");
        public ApplicationKeySetColumns Created => new ApplicationKeySetColumns("Created");


		public Type DaoType => typeof(ApplicationKeySet);

		public string Operator { get; set; }

        public override string ToString()
        {
            return base.ColumnName;
        }
	}
}