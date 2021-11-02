using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Data.Repositories
{
    public interface IHasCompositeKey : IHasKeyHash
    {
        HashAlgorithms CompositeKeyAlgorithm { get; set; }
        string CompositeKey { get; set; }
        ulong CompositeKeyId { get; set; }

        bool Exists(IRepository repo, out RepoData existing);
        RepoData Save(IRepository repo);
    }
}
