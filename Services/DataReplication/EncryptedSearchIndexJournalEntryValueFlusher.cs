using Bam.Net.Encryption;
using Bam.Net.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Services.DataReplication
{
    public class EncryptedSearchIndexJournalEntryValueFlusher : IJournalEntryValueFlusher
    {
        public EncryptedSearchIndexJournalEntryValueFlusher(IKeyValueStore publicKeyValueStore, IKeySet keyset = null, ILogger logger = null)
        {
            Logger = logger ?? Log.Default;
            KeySet = keyset ?? Bam.Net.Encryption.Data.Files.KeySetFile.ForApplication;
            PrivateKeyValueStore = new FileSystemKeyValueStore(Logger);
            EncryptedSearchIndex = new EncryptedSearchIndex(publicKeyValueStore, PrivateKeyValueStore, KeySet, logger);
        }

        public ILogger Logger { get; set; }
        public IKeySet KeySet { get; }
        public FileSystemKeyValueStore PrivateKeyValueStore { get; internal set; }
        public EncryptedSearchIndex EncryptedSearchIndex { get; }

        EncryptedSearchIndexJournalEntryValueLoader _loader;
        public EncryptedSearchIndexJournalEntryValueLoader GetLoader()
        {
            if (_loader == null)
            {
                _loader = new EncryptedSearchIndexJournalEntryValueLoader(EncryptedSearchIndex.PublicKeyValueStore, KeySet, Logger);
            }
            return _loader;
        }

        public FileInfo Flush(Journal journal, JournalEntry entry)
        {
            try
            {
                string key = entry.GetKey();
                EncryptedSearchIndex.IndexValue(key, entry.Value);
                return new FileInfo(PrivateKeyValueStore.GetFilePath(key));
            }
            catch (Exception ex)
            {
                Logger.AddEntry("{0}: Exception flushing journal entry: {1}", ex, nameof(EncryptedSearchIndexJournalEntryValueFlusher), ex.Message);
                return null;
            }
        }
    }
}
