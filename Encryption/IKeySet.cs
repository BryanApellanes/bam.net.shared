using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Encryption
{
    /// <summary>
    /// An Aes key, iv and asymmetric key pem string.
    /// </summary>
    public interface IKeySet
    {
        string RsaKey { get; set; }
        string AesKey { get; set; }
        string AesIV { get; set; }

        RsaKeyLength RsaKeyLength { get; set; }

        string GetSecret();

        /// <summary>
        /// Encrypt the specified value with the aes key and iv of the current key set.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Encrypt(string value);

        string Decrypt(string base64EncodedValue);

        string AsymmetricEncrypt(string plainText, IAsymmetricBlockCipher engine = null);

        /// <summary>
        /// Use private key to decrypt, same as PrivateKeyDecrypt.
        /// </summary>
        /// <param name="cipher">The cipher.</param>
        /// <param name="engine">The engine.</param>
        /// <returns></returns>
        string AsymmetricDecrypt(string cipher, IAsymmetricBlockCipher engine = null);

        /// <summary>
        /// Use public key to encrypt, same as AsymmetricEncrypt.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="engine">The engine.</param>
        /// <returns></returns>
        string PublicKeyEncrypt(string plainText, IAsymmetricBlockCipher engine = null);

        string PrivateKeyDecrypt(string cipher, IAsymmetricBlockCipher engine = null);

        AesKeyVectorPair GetAesKey();

        AsymmetricCipherKeyPair GetAsymmetricKeys();        

    }
}
