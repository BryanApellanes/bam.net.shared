using Bam.Net.ServiceProxy.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    [PipelineFactoryTransformerName("rsa")]
    public class RsaByteTransformer : ValueTransformer<byte[], byte[]>
    {
        public RsaByteTransformer(Func<RsaPublicKey> publicKeyProvider, Func<RsaPublicPrivateKeyPair> privateKeyProvider)
        {
            this.KeyProvider = publicKeyProvider;
            this.RsaByteReverseTransformer = new RsaByteReverseTransformer(this, privateKeyProvider);
        }

        public RsaByteTransformer(RsaPublicPrivateKeyPair rsaPublicPrivateKeyPair) : this(() => rsaPublicPrivateKeyPair.GetRsaPublicKey(), () => rsaPublicPrivateKeyPair)
        { 
        }

        [PipelineFactoryConstructor]
        public RsaByteTransformer(IRsaPublicKeySource rsaKeySource, IRsaKeySource rsaPublicPrivateKeySource) : this(() => rsaKeySource.GetRsaPublicKey(), () => rsaPublicPrivateKeySource.GetRsaKey())
        { 
        }

        public RsaByteTransformer(Func<RsaPublicKey> publicKeyProvider)
        {
            this.KeyProvider = publicKeyProvider;
        }

        public RsaByteTransformer(IRsaPublicKeySource rsaPublicKeySource) : this(() => rsaPublicKeySource.GetRsaPublicKey())
        {
        }

        public Func<RsaPublicKey> KeyProvider { get; set; }

        public RsaByteReverseTransformer RsaByteReverseTransformer { get; set; }

        public override byte[] ReverseTransform(byte[] cipherBytes)
        {
            return GetReverseTransformer().ReverseTransform(cipherBytes);
        }

        public override byte[] Transform(byte[] plainData)
        {
            Args.ThrowIfNull(KeyProvider, "KeyProvider");
            RsaPublicKey rsaKey = KeyProvider();
            return rsaKey.EncryptBytes(plainData);
        }

        public override IValueReverseTransformer<byte[], byte[]> GetReverseTransformer()
        {
            Args.ThrowIfNull(this.RsaByteReverseTransformer);
            return this.RsaByteReverseTransformer;
        }
    }
}
