using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace Bam.Net.Encryption
{
    public static class ManagedVaults
    {
        public static Dictionary<string, ManagedVault> Named
        {
            get
            {
               return All.ToDictionary<ManagedVault>(mv => mv.Name, mv => (ManagedVault)mv);
            }
        }

        public static List<ManagedVault> All { get; } = Load();

        public static string PlainTextDirectory => Path.Combine(BamProfile.VaultsPath, "plaintext");

        private static List<ManagedVault> _managedVaults;
        public static List<ManagedVault> Load()
        {
            if (_managedVaults == null)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(PlainTextDirectory);
                List<ManagedVault> results = new List<ManagedVault>();
                // for all json and yaml files in plaintext directory
                foreach (FileInfo fileInfo in directoryInfo.GetFiles(new string[] {"*.yaml", "*.json"},
                    SearchOption.AllDirectories))
                {
                    results.Add(new ManagedVault(fileInfo));
                }

                _managedVaults = results;
            }
            return _managedVaults;
        }
    }
}