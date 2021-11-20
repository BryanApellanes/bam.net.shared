﻿using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Services.DataReplication
{
    /// <summary>
    /// A class that provides a universal mapping between numeric (ulong) id values and Types.
    /// The id of a type uniquely identifies a type by its fully qualified namespace name.
    /// </summary>
    public partial class TypeMap
    {
        public TypeMap() : this(SystemPaths.Current)
        {
        }

        public TypeMap(SystemPaths paths)
        {
            MappedTypes = new HashSet<Type>();
            TypeMappings = new ConcurrentDictionary<ulong, string>();
            PropertyMappings = new ConcurrentDictionary<ulong, string>();
            
            if(paths != null)
            {
                Directory = new DirectoryInfo(Path.Combine(paths.Data.AppData));
            }
        }
        
        protected internal DirectoryInfo Directory { get; set; }
        
        public string Save()
        {
            string path = Path.Combine(Directory.FullName, nameof(TypeMap));
            this.ToJsonFile(path);
            return path;
        }

        public static TypeMap Load(string path)
        {
            TypeMap typeMap = path.FromJsonFile<TypeMap>();
            typeMap.Directory = new FileInfo(path).Directory;
            return typeMap;
        }

        protected HashSet<Type> MappedTypes { get; set; }

        public ConcurrentDictionary<ulong, string> TypeMappings { get; set; }
        public ConcurrentDictionary<ulong, string> PropertyMappings { get; set; }

        public void AddMapping(CompositeKeyAuditRepoData instance)
        {
            Args.ThrowIfNull(instance, "instance");
            Type type = instance.GetType();
            if (MappedTypes.Contains(type))
            {
                return;
            }
            AddTypeMapping(type);
            foreach (PropertyInfo property in type.GetProperties())
            {
                AddPropertyMapping(property);
            }
            MappedTypes.Add(type);
        }
        /// <summary>
        /// Gets the name of the type using TypeMappings, if the mapping is not found the string representation of 
        /// the specified typeId is returned.
        /// </summary>
        /// <param name="typeId">The type identifier.</param>
        /// <returns></returns>
        public string GetTypeName(ulong typeId)
        {
            if(TypeMappings.TryGetValue(typeId, out string value))
            {
                return value;
            }
            return typeId.ToString();
        }

        public string GetTypeName(CompositeKeyAuditRepoData instance)
        {
            return GetTypeName(GetTypeId(instance));
        }

        public string GetTypeShortName(CompositeKeyAuditRepoData instance)
        {
            return GetTypeShortName(GetTypeId(instance));
        }

        public string GetTypeShortName(ulong typeId)
        {
            return GetTypeName(typeId).DelimitSplit(".").Last();
        }

        /// <summary>
        /// Gets the name of the property using PropertyMappings, if the mapping is not found the string representation
        /// of the specified property is returned.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <returns></returns>
        public string GetPropertyName(ulong propertyId)
        {
            if(PropertyMappings.TryGetValue(propertyId, out string value))
            {
                return value;
            }
            return propertyId.ToString();
        }

        public string GetPropertyShortName(ulong propertyId)
        {
            string propAndType = GetPropertyName(propertyId).DelimitSplit(".").Last();
            return propAndType.DelimitSplit("__").First();
        }
        
        protected ulong AddPropertyMapping(PropertyInfo property)
        {
            ulong key = GetPropertyId(property, out string name);
            PropertyMappings.TryAdd(key, name);
            return key;
        }

        protected ulong AddTypeMapping(Type type)
        {
            ulong key = GetTypeId(type, out string name);
            TypeMappings.TryAdd(key, name);
            return key;
        }

        public static PropertyInfo[] GetTypeProperties<T>() where T: CompositeKeyAuditRepoData, new()
        {
            GetTypeId(new T(), out object ignore, out Type dynamicType);
            return dynamicType.GetProperties();
        }

        public static ulong GetTypeId(CompositeKeyAuditRepoData instance)
        {
            return GetTypeId(instance, out object ignore1, out Type ignore2);
        }

        public static ulong GetTypeId(Type type)
        {
            return GetTypeId(type, out string ignore);
        }

        public static ulong GetTypeId(Type type, out string name)
        {
            name = NormalizeName(type);
            // TODO: change this implementation to account for property definitions, this allows definitions to be valid across updates to type schemas
            // TODO: define TypeMigration mapping old Type to new Type after change of schema
            return name.ToSha256ULong();
        }

        public static ulong GetPropertyId(PropertyInfo prop)
        {
            return GetPropertyId(prop, out string ignore);
        }

        public static ulong GetPropertyId(PropertyInfo prop, out string name)
        {
            name = NormalizeName(prop);
            return name.ToSha256ULong();
        }

        public static ulong GetTypeId(string typeNamespace, string typeName)
        {
            return $"{typeNamespace}.{typeName}".ToSha256ULong();
        }

        public static ulong GetPropertyId(string typeNamespace, string typeName, string propertyName)
        {
            return $"{typeNamespace}.{typeName}.{propertyName}__object".ToSha256ULong();
        }

        private static string NormalizeName(Type type)
        {
            return $"{type.Namespace}.{type.Name}";
        }

        private static string NormalizeName(PropertyInfo prop)
        {
            return $"{prop.DeclaringType.Namespace}.{prop.DeclaringType.Name}.{prop.Name}__{prop.PropertyType.Name}";
        }
    }
}
