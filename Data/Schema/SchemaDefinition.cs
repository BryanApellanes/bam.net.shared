/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Bam.Net;
using Bam.Net.Data;
using Newtonsoft.Json;
using Bam.Net.Configuration;
using Bam.Net.Data.Repositories;
using YamlDotNet.Serialization;

namespace Bam.Net.Data.Schema
{
    public class SchemaDefinition
    {
        Dictionary<string, Table> _tables = new Dictionary<string, Table>();
        Dictionary<string, ColumnAttribute> _columns = new Dictionary<string, ColumnAttribute>();

        public SchemaDefinition()
        {
            this.Name = "Default";
            this.DbType = "UnSpecified";
        }
        public SchemaDefinition(string name): this()
        {
            Name = name;
            File = $"{RuntimeSettings.ProcessDataFolder}\\{name}_schema_definition.json";
        }
        /// <summary>
        /// Gets or sets the type of the database that this SchemaDefinition was
        /// extracted from.  May be null.
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// Gets or sets the name of the current SchemaDefinition.
        /// </summary>
        public string Name { get; set; }

        FileInfo _file;
        [Exclude]
        public string File
        {
            get
            {
                if (_file != null)
                {
                    return _file.FullName;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _file = new FileInfo(Path.Combine(RuntimeSettings.ProcessDataFolder, this.Name));
                }
                else
                {
                    _file = new FileInfo(value);
                    if (!_file.Directory.Exists)
                    {
                        _file.Directory.Create();
                    }
                }
            }
        }

        public void RemoveTable(Table table)
        {
            RemoveTable(table.Name);
        }

        public void RemoveTable(string tableName)
        {
            if (this._tables.ContainsKey(tableName))
            {
                this._tables.Remove(tableName);
            }
        }

        public Table[] Tables
        {
            get
            {
				List<Table> tables = new List<Table>();
				tables.AddRange(this._tables.Values);
				return tables.ToArray();
            }
            set
            {
                this._tables.Clear();
                foreach (Table table in value)
                {
                    if (string.IsNullOrEmpty(table.ConnectionName))
                    {
                        table.ConnectionName = this.Name;
                    }
					if (!this._tables.ContainsKey(table.Name))
					{
						this._tables.Add(table.Name, table);
					}
					else
					{
						throw Args.Exception<InvalidOperationException>("Table named {0} defined more than once", table.Name);
					}
                }
            }
        }

        internal Table GetTable(string tableName)
        {
            Table table = null;
			if (this._tables.ContainsKey(tableName))
			{
				table = this._tables[tableName];
			}
            return table;
        }

        readonly List<ForeignKeyColumn> _foreignKeys = new List<ForeignKeyColumn>();
        public ForeignKeyColumn[] ForeignKeys
        {
            get => this._foreignKeys.ToArray();
            set
            {
                this._foreignKeys.Clear();
                this._foreignKeys.AddRange(value);
            }
        }

        Dictionary<string, XrefTable> _xrefs = new Dictionary<string, XrefTable>();
        public XrefTable[] Xrefs
        {
            get => _xrefs.Values.ToArray();
            set
            {
                this._xrefs = value.ToDictionary<XrefTable, string>(x => x.Name); // to Dictionary key by name
            }
        }

        public XrefInfo[] LeftXrefsFor(string tableName)
        {
            return (from xref in Xrefs
                    where xref.Left.Equals(tableName)
                    select xref).Select(x => new XrefInfo(tableName, x.Name, x.Right)).ToArray();
        }

        public XrefInfo[] RightXrefsFor(string tableName)
        {
            return (from xref in Xrefs
                    where xref.Right.Equals(tableName)
                    select xref).Select(x => new XrefInfo(tableName, x.Name, x.Left)).ToArray();
        }

        internal XrefTable GetXref(string tableName)
        {
            XrefTable result = null;
            if (this._xrefs.ContainsKey(tableName))
            {
                result = this._xrefs[tableName];
            }

            return result;
        }

        public SchemaManagerResult AddXref(XrefTable xref)
        {
            SchemaManagerResult r = new SchemaManagerResult($"XrefTable {xref.Name} was added.");
            try
            {
                xref.ConnectionName = this.Name;
                if (_xrefs.ContainsKey(xref.Name))
                {
                    _xrefs[xref.Name] = xref;
                    r.Message = $"XrefTable {xref.Name} was updated.";
                }
                else
                {
                    _xrefs.Add(xref.Name, xref);
                }
            }
            catch (Exception ex)
            {
                SetErrorDetails(r, ex);
            }

            return r;
        }

        public void RemoveXref(string name)
        {
            if (_xrefs.ContainsKey(name))
            {
                _xrefs.Remove(name);
            }
        }

        public void RemoveXref(XrefTable xrefTable)
        {
            if (_xrefs.ContainsKey(xrefTable.Name))
            {
                _xrefs.Remove(xrefTable.Name);
            }
        }

        public SchemaManagerResult AddTable(Table table)
        {
            SchemaManagerResult r = new SchemaManagerResult($"Table {table.Name} was added.");
            try
            {
                table.ConnectionName = this.Name;
                if (this._tables.ContainsKey(table.Name))
                {
                    this._tables[table.Name] = table;
                    r.Message = $"Table {table.Name} was updated.";
                }
                else
                {
                    this._tables.Add(table.Name, table);
                }
            }
            catch (Exception ex)
            {
                SetErrorDetails(r, ex);
            }

            return r;
        }

        public SchemaManagerResult AddForeignKey(ForeignKeyColumn fk)
        {
            SchemaManagerResult r = new SchemaManagerResult($"ForeignKey {fk.ReferenceName} was added.");
            try
            {
                if (!this._foreignKeys.Contains(fk))
                {
                    this._foreignKeys.Add(fk);
                }
                else
                {
                    ForeignKeyColumn existing = (from efk in this._foreignKeys
                                                 where efk.Equals(fk)
                                                 select efk).FirstOrDefault();

                    existing.AllowNull = fk.AllowNull;
                    existing.DbDataType = fk.DbDataType;
                    existing.Key = fk.Key;
                    existing.MaxLength = fk.MaxLength;
                    existing.Name = fk.Name;
                    existing.ReferencedKey = fk.ReferencedKey;
                    existing.ReferencedTable = fk.ReferencedTable;
                    existing.ReferenceName = fk.ReferenceName;
                    existing.TableName = fk.TableName;                    
                }
            }
            catch (Exception ex)
            {
                SetErrorDetails(r, ex);
            }

            return r;
        }
        
        private void SetErrorDetails(SchemaManagerResult r, Exception ex)
        {
            this.LastException = ex;
            r.Message = ex.Message;
            r.Success = false;
            r.StackTrace = ex.StackTrace;
        }
        
        /// <summary>
        /// The most recent exception that occurred after trying to add a table
        /// or a foreign key
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        [XmlIgnore]
        public Exception LastException
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Loads a SchemaDefinition from the specified file, the file
        /// is created if it doesn't exist.
        /// </summary>
        /// <param name="schemaFile"></param>
        /// <returns></returns>
        public static SchemaDefinition Load(string schemaFile)
        {
            SchemaDefinition schema = new SchemaDefinition {File = schemaFile};
            if (System.IO.File.Exists(schemaFile)) 
			{
	            schema = schemaFile.FromJsonFile<SchemaDefinition>();
            }
            else
            {
                Save(schema);
            }
            return schema;
        }

        /// <summary>
        /// Serializes the current SchemaDefinition as json to the
        /// file specified in the File property
        /// </summary>
        public void Save()
        {
            Save(this);
        }

        /// <summary>
        /// Serializes the current SchemaDefinition as json to the
        /// specified filePath
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            this.File = filePath;
            Save(this);
        }

        public SchemaDefinition CombineWith(SchemaDefinition schemaDefinition)
        {
            foreach (Table table in schemaDefinition.Tables)
            {
                AddTable(table);
            }

            foreach(ForeignKeyColumn foreignKey in schemaDefinition.ForeignKeys)
            {
                AddForeignKey(foreignKey);
            }
            
            foreach (XrefTable xref in schemaDefinition.Xrefs)
            {
                AddXref(xref);
            }

            return this;
        }
        
        static readonly object _saveLock = new object();
        private static void Save(SchemaDefinition schema)
        {
            lock (_saveLock)
            {
                schema.ToJsonFile(schema.File);
            }
        }
    }
}
