using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Web
{
    public class EncryptedHttpClientResponse : HttpClientResponse
    {
        public EncryptedHttpClientResponse(IAesKeySource clientKeySource, string cipher)
        {
            this.AesKeySource = clientKeySource;
            this.Cipher = cipher;
        }

        protected IAesKeySource AesKeySource { get; set; }
        protected string Cipher{ get; set; }

        public override string Content 
        {
            get
            {
                return AesKeySource.GetAesKey().Decrypt(Cipher);
            }
        }
    }
}
