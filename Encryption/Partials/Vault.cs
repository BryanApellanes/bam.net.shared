/*
	Copyright © Bryan Apellanes 2015  
*/
// Model is Table
using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Bam.Net;
using Bam.Net.Data;
using Bam.Net.Data.Qi;
using Bam.Net.Logging;
using Bam.Net.Configuration;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Engines;
using System.Linq;
using System.Security.Cryptography;

namespace Bam.Net.Encryption
{
    /// <summary>
    /// An encrypted key value store used to prevent
    /// casual access to sensitive data like passwords.  Encrypted data is stored
    /// in a sqlite file by default or a Database you specify.
    /// </summary>
	public partial class Vault
	{
        public Dictionary<string, string> ExportValues()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach(string key in Keys)
            {
                result.Add(key, this[key]);
            }
            return result;
        }

        public void ImportValues(Dictionary<string, string> values)
        {
            foreach(string key in values.Keys)
            {
                this[key] = values[key];
            }
        }

        public string RuntimePassword { internal get; set; }
        
        /// <summary>
        /// Exports the VaultKey from the current vault.  The VaultKey consists of
        /// an Rsa key pair and an Aes password used to encrypt the current vault.
        /// VaultKey is null after this operation.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public VaultKeyInfo ExportKey(Database db = null)
        {
            db = db ?? Database;
            VaultKey key = VaultKeysByVaultId.FirstOrDefault();
            _vaultKey = null;
            _items = null;
            VaultKeysByVaultId.Delete(db);
            ChildCollections.Clear();
            VaultKeyInfo result = key.CopyAs<VaultKeyInfo>();
            return result;
        }

        /// <summary>
        /// Imports the specified key.  VaultKey is not null after this operation.
        /// </summary>
        /// <param name="keyInfo"></param>
        /// <param name="db"></param>
        public void ImportKey(VaultKeyInfo keyInfo, Database db = null)
        {
            VaultKey key = VaultKeysByVaultId.AddChild();
            key.RsaKey = keyInfo.RsaKey;
            key.Password = keyInfo.Password;
            key.Save(db);
            _items = null;
            ChildCollections.Clear();
        }

        static Database _profileVaultDatabase;
        static object _profileVaultDatabaseSync = new object();
        public static Database ProfileVaultDatabase
        {
            get => _profileVaultDatabaseSync.DoubleCheckLock(ref _profileVaultDatabase, InitializeProfileVaultDatabase);
            set => _profileVaultDatabase = value;
        }
        
        static Database _systemVaultDatabase;
        static object _systemVaultDatabaseSync = new object();
        public static Database SystemVaultDatabase
        {
            get => _systemVaultDatabaseSync.DoubleCheckLock(ref _systemVaultDatabase, InitializeSystemVaultDatabase);
            set => _systemVaultDatabase = value;
        }

        static Database _applicationVaultDatabase;
        static object _applicationVaultDatabaseSync = new object();
        public static Database ApplicationVaultDatabase
        {
            get => _applicationVaultDatabaseSync.DoubleCheckLock(ref _applicationVaultDatabase, InitializeApplicationVaultDatabase);
            set => _applicationVaultDatabase = value;
        }

        internal static Database InitializeSystemVaultDatabase()
        {            
            string path = Path.Combine(BamHome.DataPath, $"System.vault.sqlite");
            return InitializeVaultDatabase(path, Log.Default);
        }
        
        internal static Database InitializeProfileVaultDatabase()
        {            
            string path = Path.Combine(BamProfile.DataPath, $"Profile.vault.sqlite");
            return InitializeVaultDatabase(path, Log.Default);
        }

        internal static Database InitializeApplicationVaultDatabase()
        {
            string appName = DefaultConfigurationApplicationNameProvider.Instance.GetApplicationName();
            string path = Path.Combine(BamHome.DataPath, $"Application_{appName}.vault.sqlite");
            return InitializeVaultDatabase(path, Log.Default);
        }

        public static Database InitializeVaultDatabase(string filePath, ILogger logger = null)
        {
            Database db = null;

            VaultDatabaseInitializer initializer = new VaultDatabaseInitializer(filePath);
            DatabaseInitializationResult result = initializer.Initialize();
            if (!result.Success)
            {
                logger.AddEntry(result.Exception.Message, result.Exception);
            }

            db = result.Database;

            return db;
        }

        protected internal static string Password => "287802b5ca734821";

        static Vault _profileVault;
        static object _profileVaultSync = new object();
        public static Vault Profile
        {
            get
            {
                return _profileVaultSync.DoubleCheckLock(ref _profileVault, () => Retrieve(ProfileVaultDatabase, "Profile", Password));
            }
        }
        
        static Vault _systemVault;
        static object _systemVaultSync = new object();
        public static Vault System
        {
            get
            {
                return _systemVaultSync.DoubleCheckLock(ref _systemVault, () => Retrieve(SystemVaultDatabase, "System", Password));
            }
        }

        static Vault _appVault;
        static object _appVaultSync = new object();
        public static Vault Application
        {
            get
            {
                string appName = DefaultConfigurationApplicationNameProvider.Instance.GetApplicationName();
                return _appVaultSync.DoubleCheckLock(ref _appVault, () => Retrieve(ApplicationVaultDatabase, appName, Password));
            }
        }

        /// <summary>
        /// Loads the default vault for the current application.  The default path is
        /// {RuntimeSettings.AppDataFolder}\{ApplicationName}.vault.sqlite.  Paths.AppData
        /// references RuntimeSettings.AppDataFolder so the former can be used as shorthand
        /// for the latter.  Setting Paths.AppData will effectively redirect where the vault
        /// is loaded from.
        /// </summary>
        /// <returns></returns>
        public static Vault Load()
        {
            return Load(new VaultInfo());
        }

        /// <summary>
        /// Loads the specified vault name.  The default path is
        /// {RuntimeSettings.AppDataFolder}\{vaultName}.vault.sqlite.  Paths.AppData
        /// references RuntimeSettings.AppDataFolder so the former can be used as shorthand
        /// for the latter.  Setting Paths.AppData will effectively redirect where the vault
        /// is loaded from.
        /// </summary>
        /// <param name="vaultName">Name of the vault.</param>
        /// <returns></returns>
        public static Vault Load(string vaultName)
        {
            return Load(new VaultInfo(vaultName));
        }

        public static Vault Load(VaultInfo vaultInfo)
        {
            return Load(vaultInfo.FilePath, vaultInfo.Name);
        }

        public static Vault Load(string filePath, string vaultName)
        {
            return Load(new FileInfo(filePath), vaultName, out VaultDatabase ignore);
        }

        public static Vault Load(string filePath, string vaultName, out VaultDatabase vaultDb)
        {
            return Load(new FileInfo(filePath), vaultName, out vaultDb);            
        }

        public static Vault Load(FileInfo file, string vaultName)
        {
            return Load(file, vaultName, out VaultDatabase ignore);
        }

        public static Vault Load(FileInfo file, string vaultName, out VaultDatabase vaultDb)
        {
            return Load(file, vaultName, "".RandomLetters(16), out vaultDb); // password will only be used if the file doesn't exist
        }

        static Dictionary<string, Vault> _loadedVaults = new Dictionary<string, Vault>();
        static object _loadedVaultsLock = new object();
        public static Vault Load(FileInfo file, string vaultName, string password, ILogger logger = null)
        {
            return Load(file, vaultName, password, out VaultDatabase ignore, logger);
        }

        public static Vault Load(FileInfo file, string vaultName, string password, out VaultDatabase vaultDb, ILogger logger = null)
        {
            string key = $"{file.FullName}.{vaultName}";
            lock (_loadedVaultsLock)
            {
                if (!_loadedVaults.ContainsKey(key))
                {
                    if (logger == null)
                    {
                        logger = Log.Default;
                    }
                    Database db = InitializeVaultDatabase(file.FullName, logger);
                    db.SelectStar = true;
                    _loadedVaults.Add(key, Retrieve(db, vaultName, password));
                }
            }
            vaultDb = _loadedVaults[key].Database;
            return _loadedVaults[key];
        }

        public Vault Rename(string newName)
        {
            Name = newName;
            Save(Database);
            return this;
        }

        /// <summary>
        /// Get the vault with the specified name from the SystemVaultDatabase.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Vault Retrieve(string name)
        {
            return Retrieve(SystemVaultDatabase, name, Secure.RandomString());
        }

        /// <summary>
        /// Get the Vault with the specified name using the
        /// specified password to create it if it doesn't exist
        /// in the SystemVaultDatabase.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Vault Retrieve(string name, string password)
        {
            return Retrieve(SystemVaultDatabase, name, password);
        }

        /// <summary>
        /// Get a Vault from the specified database with the
        /// specified name using the specified password to
        /// create it if it doesn't exist.  Will return null
        /// if password is not specified and the vault 
        /// doesn't exist in the specified database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="vaultName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Vault Retrieve(Database database, string vaultName, string password = null)
        {
            database.TryEnsureSchema<Vault>();
            Vault result = Vault.OneWhere(c => c.Name == vaultName, database);
            if (result == null && !string.IsNullOrEmpty(password))
            {
                result = Create(database, vaultName, password);
            }

            result?.Decrypt();
            result.RuntimePassword = password;
            return result;
        }

        /// <summary>
        /// Create a vault in the specified file by the 
        /// specified name.  If the vault already exists
        /// in the specified file the existing vault
        /// is returned
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Vault Create(FileInfo file, string name)
        {
            string password = GeneratePassword();
            return Create(file, name, password);
        }

        public static Vault Create(FileInfo file, string name, string password, RsaKeyLength rsaKeyLength = RsaKeyLength._4096)
        {
            Database db = InitializeVaultDatabase(file.FullName, Log.Default);
            return Create(db, name, password, rsaKeyLength);
        }

        public static Vault Create(string name)
        {
            string password = GeneratePassword();
            return Create(name, password);
        }

        public static string GeneratePassword(int length = 64)
        {
            SecureRandom random = new SecureRandom();
            string password = random.GenerateSeed(length).ToBase64();
            return password;
        }

        public static Vault Create(string name, string password, RsaKeyLength rsaKeyLength = RsaKeyLength._4096)
        {
            Database db = InitializeSystemVaultDatabase();
            return Create(db, name, password, rsaKeyLength);
        }

        /// <summary>
        /// Create a Vault in the specified database by the specified
        /// name using the specified password to create it if it
        /// doesn't exist
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <param name="password">The password that is used as a key if the vault of the specified name does not exist.</param>
        /// <param name="rsaKeyLength"></param>
        /// <returns></returns>
        public static Vault Create(Database database, string name, string password, RsaKeyLength rsaKeyLength = RsaKeyLength._4096)
        {
            Vault result = Vault.OneWhere(c => c.Name == name, database);
            if (result == null)
            {
                result = new Vault
                {
                    Name = name
                };
                result.Save(database);
            }

            if (result.VaultKey == null)
            {
                SetVaultKey(result, password, rsaKeyLength, database);
            }

            return result;
        }

        public void ResetVaultKey(string password, RsaKeyLength rsaKeyLength = RsaKeyLength._4096)
        {
            // get all the current decrypted values
            // delete all the current encrypted values from the database
            // set the VaultKey
            // add all the decrypted values back using the new key
            throw new NotImplementedException();
        }
        
        public void SetVaultKey(string password, RsaKeyLength rsaKeyLength = RsaKeyLength._4096)
        {
            SetVaultKey(this, password, rsaKeyLength);
        }

        public static void SetVaultKey(Vault vault, string password, RsaKeyLength rsaKeyLength)
        {
            Args.ThrowIfNull(vault.Database, "vault.Database");
            SetVaultKey(vault, password, rsaKeyLength, vault.Database);
        }
        
        public static void SetVaultKey(Vault vault, string password, RsaKeyLength rsaKeyLength, Database database)
        {
            VaultKey key = vault.VaultKeysByVaultId.JustOne(database, false);
            AsymmetricCipherKeyPair keys = Rsa.GenerateKeyPair(rsaKeyLength);
            key.RsaKey = keys.ToPem();
            key.Password = password.EncryptWithPublicKey(keys);
            key.Save(database);
        }

        public string ConnectionString => Database.ConnectionString;

        Dictionary<string, DecryptedVaultItem> _items;
        object _itemsLock = new object();
        protected Dictionary<string, DecryptedVaultItem> Items
        {
            get
            {
                return _itemsLock.DoubleCheckLock(ref _items, () => new Dictionary<string, DecryptedVaultItem>());
            }
        }

        private bool Decrypt()
        {
            _items = null; // will cause it to reinitialize above
            if(VaultKey != null)
            {
                string password = VaultKey.PrivateKeyDecrypt(VaultKey.Password);
                VaultItemsByVaultId.Reload();
                VaultItemsByVaultId.Each(item =>
                {
                    string key = item.Key.AesPasswordDecrypt(password);
                    DecryptedVaultItem value = new DecryptedVaultItem(item, VaultKey);
                    Items.Add(key, value);
                });
                return true;
            }
            return false;
        }

        private VaultKey _vaultKey;
		public VaultKey VaultKey => _vaultKey = _vaultKey ?? VaultKeysByVaultId.FirstOrDefault();

        public string[] Keys
        {
            get
            {
                Decrypt();
                string[] keys = new string[Items.Keys.Count];
                Items.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        public bool HasKey(string key)
        {
            return HasKey(key, out string ignore);
        }

        public bool HasKey(string key, out string value)
        {
            value = Get(key);
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Set a key value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, string value)
        {
            this[key] = value;
        }

        public string Get(string key)
        {
            return this[key];
        }

        object writeLock = new object();
        public string this[string key]
        {
            get
            {
                if (Items.ContainsKey(key))
                {
                    return Items[key].Value;
                }
                else
                {
                    Decrypt();
                    if (Items.ContainsKey(key))
                    {
                        return Items[key].Value;
                    }
                }

                return null;
            }
            set
            {
                lock (writeLock)
                {
                    if (VaultKey == null)
                    {
                        throw new VaultKeyNotSetException(this);
                    }
                    if (Decrypt())
                    {
                        string val = value ?? "null";
                        if (Items.ContainsKey(key))
                        {
                            Items[key].Value = val;
                        }
                        else
                        {
                            VaultItem item = VaultItemsByVaultId.AddChild();
                            string password = VaultKey.PrivateKeyDecrypt(VaultKey.Password);
                            item.Key = key.AesPasswordEncrypt(password);
                            item.Value = val.AesPasswordEncrypt(password);
                            item.Save();
                            Items[key] = new DecryptedVaultItem(item, VaultKey);
                        }
                    }
                }
            }
        }

        public bool Remove(string key)
        {
            lock (writeLock)
            {
                if (VaultKey == null)
                {
                    throw new VaultKeyNotSetException(this);
                }
                if (Decrypt())
                {
                    if (Items.ContainsKey(key))
                    {
                        return Items.Remove(key);
                    }
                }
            }

            return false;
        }
        
        public Vault Copy(FileInfo file)
        {
            return Copy(file, Name);
        }

        public Vault Copy(FileInfo file, string name)
        {
            Vault copy = Vault.Load(file, name);
            Keys.Each(key =>
            {
                copy[key] = this[key];
            });

            return copy;
        }
	}
}																								
