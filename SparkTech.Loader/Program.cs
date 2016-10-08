using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Win32;

namespace SparkTech.Loader
{
    /// <summary>
    /// The <see cref="Program"/> class
    /// </summary>
    [SuppressMessage("ReSharper", "LocalizableElement")]
    internal static class Program
    {
        /// <summary>
        /// Contains data about an updatable resource
        /// </summary>
        private struct ResourceData
        {
            /// <summary>
            /// The resource name
            /// </summary>
            public string Name;

            /// <summary>
            /// The link to cloud version of the file
            /// </summary>
            public string CloudPath;

            /// <summary>
            /// The path to the resource destination
            /// </summary>
            public string LocalPath;
        }

        /// <summary>
        /// A list containing the resource data
        /// </summary>
        private static readonly List<ResourceData> Files;

        /// <summary>
        /// The beginning for download links
        /// </summary>
        private const string BaseWebPath = "https://raw.githubusercontent.com/Wiciaki/Dependencies/master/Download/";

        /// <summary>
        /// The lowest recommended .NET Version
        /// </summary>
        private const int RecommendedDotNetVersion = 394802;

        /// <summary>
        /// Initializes static members of the <see cref="Program"/> class
        /// </summary>
        static Program()
        {
            var librariesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EloBuddy", "Addons", "Libraries");

            Directory.CreateDirectory(librariesPath);

            var libs = new List<string> { "SparkTech.SDK", "NLog", "MoreLinq" };
            Files = new List<ResourceData>(libs.Count);

            foreach (var name in libs)
            {
                var dll = name + ".dll";

                Files.Add(new ResourceData
                {
                    Name = name, CloudPath = BaseWebPath + dll, LocalPath = Path.Combine(librariesPath, dll)
                });
            }
        }

        /// <summary>
        /// The entry point of an application
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!FrameworkUpdated())
            {
                Console.Write("Recommended .NET Framework version is at least 4.6.2 which was not detected on your system. Attempting to start anyway...\n\n");
                Thread.Sleep(3000);
            }

            var dir = Directory.GetCurrentDirectory();

            var updater = Path.Combine(dir, "Updater.exe");
            {
                var info = new FileInfo(updater);

                if (info.Exists)
                {
                    info.Delete();
                }
            }
            
            string loader;

            using (var client = new WebClient())
            {
                var assemblyName = typeof(Program).Assembly.GetName();
                var web = client.DownloadString(BaseWebPath + assemblyName.Name + ".txt");

                if (assemblyName.Version < new Version(web))
                {
                    File.WriteAllBytes(updater, Properties.Resources.Updater);
                    ExitTo(updater);
                }

                // ReSharper disable once PossibleNullReferenceException
                if (!new DirectoryInfo(dir).Name.Equals("elobuddy", StringComparison.OrdinalIgnoreCase) || (loader = Directory.GetFiles(dir).Select(Path.GetFileName).SingleOrDefault(x => x.Equals("elobuddy.loader.exe", StringComparison.OrdinalIgnoreCase))) == null)
                {
                    Console.Write("This file must be placed in EloBuddy installation folder!\nPlease move this file and try again.\n");
                    Console.Read();
                    return;
                }

                Console.Write("Welcome! EloBuddy is starting soon...\n\n");

                foreach (var file in Files)
                {
                    Console.Write($@"Downloading {file.Name}...");
                    client.DownloadFile(file.CloudPath, file.LocalPath);
                    Console.WriteLine(@" done!");
                }
            }

            ExitTo(loader);
        }

        /// <summary>
        /// Exits the current application and starts another process
        /// </summary>
        /// <param name="path">The .exe file path to be opened</param>
        private static void ExitTo(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                Process.Start(path);
            }
            
            Environment.Exit(0);
        }

        /// <summary>
        /// Determines whether the current framework version is at least the recommended one
        /// </summary>
        /// <returns></returns>
        private static bool FrameworkUpdated()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
            return key != null && Convert.ToUInt32(key.GetValue("Release", 0)) > RecommendedDotNetVersion;
        }
    }
}