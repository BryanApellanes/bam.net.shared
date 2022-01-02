using Bam.Net.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption.Data
{
    public class ApplicationKeySet : KeySet, IApplicationKeySet
    {
        public ApplicationKeySet() { }
        public ApplicationKeySet(RsaKeyLength keyLength = RsaKeyLength._2048, bool init = false ) : base(keyLength)
        {
            this.RsaKeyLength = keyLength;
            this.ApplicationName = ApplicationNameProvider.Default.GetApplicationName();
            if (init)
            {
                base.Init();
            }
        }

        [CompositeKey]
        public string ApplicationName { get; set; }
    }
}
