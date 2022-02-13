using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Web
{
    public static class Headers
    {
        /// <summary>
        /// Gets the header value that identifies the current process using Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor.LocalIdentifier.
        /// </summary>
        public static string ProcessLocalIdentifier => "X-Bam-Process-Local-Id";

        /// <summary>
        /// Gets the header value that describes the current process using Bam.Net.CoreServices.ApplicationRegistration.Data.ProcessDescriptor.Current.ToString();
        /// </summary>
        public static string ProcessDescriptor => "X-Bam-Process-Descriptor";

        /// <summary>
        /// Gets the current process mode as reported by ProcessMode.Current.Mode.
        /// </summary>
        public static string ProcessMode => "X-Bam-Process-Mode";

        public static string ApplicationName => "X-Bam-AppName";

        public static string SecureChannelSessionId => "X-Bam-Secure-Channel-Session-Id";

        [Obsolete("Use SecureChannelSessionId instead")]
        public static string SecureSessionId => "X-Bam-Sps-Session-Id";

        public static string ValidationToken => "X-Bam-Validation-Token";

        public static string Signature => "X-Bam-Signature";

        public static string Timestamp => "X-Bam-Timestamp";

        public static string Padding => "X-Bam-Padding";

        /// <summary>
        /// Header used to prove that the client knows the shared secret by using 
        /// it to create a hash value that this header is set to.
        /// </summary>
        public static string KeyToken => "X-Bam-Key-Token";

        /// <summary>
        /// Header used to request a specific responder on the server
        /// handle a given request.
        /// </summary>
        public static string Responder => "X-Bam-Responder";
    }
}
