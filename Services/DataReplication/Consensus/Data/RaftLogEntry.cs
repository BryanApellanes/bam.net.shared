using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bam.Net.Data.Repositories;
using Bam.Net.Services.DataReplication.Consensus.Data.Dao.Repository;
using Bam.Net.Services.DataReplication.Data;

namespace Bam.Net.Services.DataReplication.Consensus.Data
{
    [Serializable]
    public class RaftLogEntry: CompositeKeyAuditRepoData
    {
        public RaftLogEntryState State { get; set; }

        /// <summary>
        /// The universal identifier for the data value Instance.
        /// </summary>
        [CompositeKey]
        public ulong InstanceId { get; set; }

        [CompositeKey]
        public ulong TypeId { get; set; }
        
        [CompositeKey]
        public ulong PropertyId { get; set; }
        
        [CompositeKey]
        public string Value { get; set; }
        
        public ulong GetId()
        {
            return GetULongKeyHash();
        }

        /// <summary>
        /// Ensures that the current RaftLogEntry exists in the specified repository.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public RaftLogEntry GetFromRepository(Repository repository)
        {
            return repository.GetByCompositeKey<RaftLogEntry>(this);
        } 
        
        public static IEnumerable<RaftLogEntry> FromInstance(CompositeKeyAuditRepoData instance)
        {
            if (instance is RaftLogEntry raftLogEntry)
            {
                yield return raftLogEntry;
            }
            else
            {
                Args.ThrowIfNull(instance, "instance");
                Type type = instance.GetType();
                instance.Id = instance.GetULongKeyHash();
                ulong typeId = TypeMap.GetTypeId(type);
                foreach (PropertyInfo prop in type.GetProperties()
                    .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string)).ToArray())
                {
                    yield return new RaftLogEntry()
                    {
                        InstanceId = instance.Id,
                        TypeId = typeId,
                        PropertyId = TypeMap.GetPropertyId(prop),
                        Value = prop.GetValue(instance, null).ToString()
                    };
                }
            }
        }

        public static IEnumerable<RaftLogEntry> FromDataPropertySet(DataPropertySet dataPropertySet)
        {
            foreach (DataProperty dataProperty in dataPropertySet)
            {
                yield return FromDataProperty(dataProperty);
            }
        }

        public static RaftLogEntry FromDataProperty(DataProperty dataProperty)
        {
            if (ulong.TryParse(dataProperty.InstanceIdentifier, out ulong instanceId))
            {
                return new RaftLogEntry()
                {
                    InstanceId = instanceId,
                    TypeId = TypeMap.GetTypeId(dataProperty.TypeNamespace, dataProperty.TypeName),
                    PropertyId = TypeMap.GetPropertyId(dataProperty.TypeNamespace, dataProperty.TypeName, dataProperty.Name),
                    Value = dataProperty.Value?.ToString() ?? string.Empty
                };
            }
            throw new InvalidOperationException("Unable to parse dataProperty.InstanceIdentifier, expected ulong");
        }

        public static IEnumerable<RaftLogEntry> FromSaveOperation(SaveOperation saveOperation)
        {
            return FromWriteOperation(saveOperation);
        }
        
        public static IEnumerable<RaftLogEntry> FromWriteOperation(WriteOperation writeOperation)
        {
            foreach (DataProperty dataProperty in writeOperation.Properties)
            {
                yield return RaftLogEntry.FromDataProperty(dataProperty);
            }
        }
    }
}