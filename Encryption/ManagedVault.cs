using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bam.Net.Logging;
using Renci.SshNet.Common;

namespace Bam.Net.Encryption
{
    /// <summary>
    /// Represents a managed vault.
    /// </summary>
    public class ManagedVault
    {
        /// <summary>
        /// Instantiate a managed vault from the specified plain text json or yaml file.
        /// </summary>
        /// <param name="plainTextFile"></param>
        /// <param name="password"></param>
        /// <param name="logger"></param>
        public ManagedVault(FileInfo plainTextFile, string password = null, ILogger logger = null)
        {
            password = password ?? Secure.RandomString();
            Logger = logger ?? Log.Default;
            Name = Path.GetFileNameWithoutExtension(plainTextFile.FullName);
            PlainTextDirectory = Path.Combine(BamProfile.VaultsPath, "plaintext");
            PlainTextFile = plainTextFile;
            if (!plainTextFile.Directory.FullName.Equals(PlainTextDirectory,
                StringComparison.InvariantCultureIgnoreCase))
            {
                string plainTextPath = Path.Combine(PlainTextDirectory, plainTextFile.Name);
                File.Copy(plainTextFile.FullName, plainTextPath);
                PlainTextFile = new FileInfo(plainTextPath);
            }
            VaultFile = new FileInfo(Path.Combine(BamProfile.VaultsPath, $"{Name}.vault.sqlite"));
            Vault = Vault.Load(VaultFile, Name, password, out VaultDatabase vaultDatabase, logger);
            VaultDatabase = vaultDatabase;
            _ = ProcessPlainTextFile();
        }

        public static Dictionary<string, ManagedVault> Named => ManagedVaults.Named;

        public string this[string key]
        {
            get => Vault[key];
            set => Vault[key] = value;
        }
        
        public string Name { get; }
        public string PlainTextDirectory { get; protected set; }
        public FileInfo PlainTextFile { get; protected set; }
        public FileInfo VaultFile { get; protected set; }
        public VaultDatabase VaultDatabase { get; protected set; }
        public Vault Vault { get; protected set; }

        public ILogger Logger { get; protected set; }
        private async Task ProcessPlainTextFile()
        {
            try
            {
                Dictionary<string, string> test = PlainTextFile.FromFile<Dictionary<string, string>>();
                test.Keys.Each(key => Vault.Set(key, test[key]));
            }
            catch(Exception ex)
            {
                Logger?.AddEntry("Error processing plain text file for Vault {0}: {1}", ex, Name, ex.Message);
            }
        }
    }
}