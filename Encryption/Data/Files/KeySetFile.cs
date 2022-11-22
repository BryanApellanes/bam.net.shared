using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bam.Net.Encryption.Data.Files
{
    public class KeySetFile : KeySet, IApplicationKeySet
    {
        public string ApplicationName { get; set; }

        static KeySetFile _forApplication;
        static object _forApplicationLock = new object();
        public static KeySetFile ForApplication
        {
            get
            {
                return _forApplicationLock.DoubleCheckLock(ref _forApplication, () => Load(ApplicationNameProvider.Default.GetApplicationName()));
            }
        }

        public static KeySetFile Load(string name)
        {
            FileInfo file = GetFile(name);
            if (!file.Exists)
            {
                file = New(name);
            }
            return Load(file);
        }

        public static FileInfo New(string name)
        {
            KeySetFile keyset = new KeySetFile() { ApplicationName = name };
            return keyset.SaveFile();
        }

        public FileInfo SaveFile()
        {
            FileInfo file = GetFile(ApplicationName);
            this.ToJsonFile(file);
            return file;
        }

        private static FileInfo GetFile(string name)
        {
            return new FileInfo(Path.Combine(BamHome.DataPath, $"{name}.keyset"));
        }

        public static KeySetFile Load(FileInfo file)
        {
            return file.FromJsonFile<KeySetFile>();
        }

    }
}
