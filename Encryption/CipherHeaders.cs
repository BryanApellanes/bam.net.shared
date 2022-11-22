using Bam.Net.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Encryption
{
    public static class CipherHeaders
    {
        /// <summary>
        /// Public key cipher of the ProcessLocalIdentifier.
        /// </summary>
        public static string ProcessLocalIdentifierCipher => $"{Headers.ProcessLocalIdentifier}-Cipher";

        /// <summary>
        /// Public key cipher of the ProcessDescriptor.
        /// </summary>
        public static string ProcessDescriptorCipher => $"{Headers.ProcessDescriptor}-Cipher";

        public static string ProcessModeCipher => $"{Headers.ProcessMode}-Cipher";

        public static string ApplicationNameCipher => $"{Headers.ApplicationName}-Cipher";

        /// <summary>
        /// Holds the public key encrypted hash of the unencrypted request body.
        /// </summary>
        public static string HashCipher => $"{Headers.Hash}-Cipher";

        public static string TimestampCipher => "X-Bam-Timestamp-Cipher";
    }
}
