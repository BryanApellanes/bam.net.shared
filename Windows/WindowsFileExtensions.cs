using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Windows
{
    public static class WindowsFileExtensions
    {
        public static void CopyTo(this DirectoryInfo directory, string computerName, string localPathOnRemote)
        {
            if (!localPathOnRemote.EndsWith("\\"))
            {
                localPathOnRemote += "\\";
            }

            List<Task> copyTasks = new List<Task>();
            foreach (FileInfo file in directory.GetFiles())
            {
                copyTasks.Add(Task.Run(() => file.CopyTo(computerName, localPathOnRemote)));
            }

            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                string subPath = dir.FullName.TruncateFront(directory.FullName.Length);
                copyTasks.Add(Task.Run(() => dir.CopyTo(computerName, localPathOnRemote + subPath)));
            }

            Task.WaitAll(copyTasks.ToArray());
        }

        public static void CopyTo(this FileInfo file, string computerName, string localPathOnRemote = null)
        {
            try
            {
                string adminSharePath = GetAdminSharePath(file.Name, computerName, localPathOnRemote);
                FileInfo destination = new FileInfo(adminSharePath);
                if (!destination.Directory.Exists)
                {
                    destination.Directory.Create();
                }
                file.CopyTo(adminSharePath, true);
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Exception copying file ({0}) to target computer ({1}), Path={2}", ex, file.FullName, computerName, localPathOnRemote);
            }
        }

        public static string GetAdminShareDirectoryPath(this string computerName, string localPathFormat)
        {
            return GetAdminSharePath(string.Empty, computerName, localPathFormat);
        }

        public static string GetAdminShareFilePath(this string computerName, string localPathFormat)
        {
            FileInfo file = new FileInfo(localPathFormat);
            return GetAdminSharePath(file, computerName);
        }

        public static string GetAdminSharePath(this FileInfo file, string computerName)
        {
            return GetAdminSharePath(file.Name, computerName, file.Directory.FullName);
        }

        /// <summary>
        /// Gets the admin share path.  For C:\windows\temp \\{ComputerName}\C$\Windows\temp is returned.
        /// </summary>
        /// <param name="fileName">The file.</param>
        /// <param name="computerName">Name of the computer.</param>
        /// <param name="remoteDirectory">The remote directory in local notation, for example, C:\windows\temp.</param>
        /// <returns></returns>
        public static string GetAdminSharePath(this string fileName, string computerName, string remoteDirectory)
        {
            remoteDirectory = remoteDirectory ?? "C$\\Windows\\Temp";
            string destinationFile = Path.Combine(remoteDirectory, fileName);
            if (destinationFile.Length >= 2 && destinationFile[1].Equals(':'))
            {
                StringBuilder df = new StringBuilder(destinationFile);
                df[1] = '$';
                destinationFile = df.ToString();
            }
            string adminSharePath = $"\\\\{computerName}\\{destinationFile}";
            return adminSharePath;
        }

        /// <summary>
        /// Gets the admin share directory in the format \\{computerName}\{driveLetter}$\{directoryPath}
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="computerName">Name of the computer.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Specified directoryPath not in expected format: [DriveLetter]:[path]</exception>
        public static DirectoryInfo GetAdminShareDirectory(this string directoryPath, string computerName)
        {
            if(directoryPath.Length >= 2 && directoryPath[1].Equals(':'))
            {
                StringBuilder path = new StringBuilder(directoryPath);
                path[1] = '$';
                return new DirectoryInfo($"\\\\{computerName}\\{path.ToString()}");
            }
            throw new ArgumentException("Specified directoryPath not in expected format: [DriveLetter]:[directoryPath]");
        }
    }
}