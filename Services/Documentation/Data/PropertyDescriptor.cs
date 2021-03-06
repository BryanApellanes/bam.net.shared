﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Net.Data.Repositories;

namespace Bam.Net.Services.Documentation.Data
{
    /// <summary>
    /// Describes the property of an object
    /// </summary>
    [Serializable]
    public class PropertyDescriptor: RepoData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long ObjectDescriptorId { get; set; }
        public virtual ObjectDescriptor Type { get; set; }
    }
}
