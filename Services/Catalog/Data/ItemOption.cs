using Bam.Net.Data.Repositories;
using System;

namespace Bam.Net.Services.Catalog.Data
{
    [Serializable]
    public class ItemOption: KeyedAuditRepoData
    {
        public ulong CatalogItemId { get; set; }
        public virtual CatalogItem CatalogItem { get; set; }
        
        [CompositeKey]
        public ulong CatalogKey { get; set; }
        [CompositeKey]
        public ulong ItemKey { get; set; }
        [CompositeKey]
        public string Name { get; set; }
        
        public string Description { get; set; }
        public decimal PriceDifference { get; set; }
    }
}