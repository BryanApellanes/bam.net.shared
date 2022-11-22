using Bam.Net.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Services.DataReplication
{
    /// <summary>
    /// A journal entry value loader that decrypts values after reading 
    /// files from disk.
    /// </summary>
    /// <seealso cref="Bam.Net.Services.DataReplication.JournalEntryValueLoader" />
    public class EncryptedJournalEntryValueLoader : JournalEntryValueLoader
    {
        public EncryptedJournalEntryValueLoader() : this(Bam.Net.Encryption.Data.Files.KeySetFile.ForApplication)
        { }

        public EncryptedJournalEntryValueLoader(IKeySet keySet)
        {
            KeySet = keySet;        
        }

        public IKeySet KeySet { get; set; }

        public override string LoadValue(string filePath)
        {
            string cipher = base.LoadValue(filePath);
            return KeySet.Decrypt(cipher);
        }
    }
}
