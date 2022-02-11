using Bam.Net.ServiceProxy.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public class RsaByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public RsaByteTransformer(Func<RsaPublicPrivateKeyPair> keyProvider)
        {
            this.KeyProvider = keyProvider;
            this.RsaByteReverseTransformer = new RsaByteReverseTransformer(this);
        }

        public RsaByteTransformer(RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair) : this(() => rsaPublicPrivateKeyPair)
        { 
        }

        public RsaByteTransformer(IRsaKeySource rsaKeySource) : this(() => rsaKeySource.GetRsaKey())
        { 
        }

        public Func<RsaPublicPrivateKeyPair> KeyProvider { get; set; }

        public RsaByteReverseTransformer RsaByteReverseTransformer { get; set; }

        public override byte[] ReverseTransform(byte[] cipherBytes)
        {
            return GetReverseTransformer().ReverseTransform(cipherBytes);
        }

        public override byte[] Transform(byte[] plainData)
        {
            Args.ThrowIfNull(KeyProvider, "KeyProvider");
            RsaPublicPrivateKeyPair rsaKey = KeyProvider();
            return rsaKey.EncryptBytes(plainData);
        }

        public override IValueReverseTransformer<byte[], byte[]> GetReverseTransformer()
        {
            return this.RsaByteReverseTransformer;
        }
    }
}
