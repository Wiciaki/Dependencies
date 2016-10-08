namespace SparkTech.Loader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;

    using Microsoft.Win32;

    /// <summary>
    /// The <see cref="Program"/> class
    /// </summary>
    [SuppressMessage("ReSharper", "LocalizableElement")]
    internal static class Program
    {
        /// <summary>
        /// Allocates a console to the current thread
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        /// <summary>
        /// Holds data for a specified resource
        /// </summary>
        private class ResourceData
        {
            /// <summary>
            /// The resource name
            /// </summary>
            public string Name;

            /// <summary>
            /// The download link for a resource
            /// </summary>
            public string CloudPath;

            /// <summary>
            /// The destination of a resource
            /// </summary>
            public string LocalPath;
        }

        /// <summary>
        /// The resource data
        /// </summary>
        private static readonly List<ResourceData> Resources;

        /// <summary>
        /// The beginning for download links
        /// </summary>
        private const string BaseWebPath = "https://raw.githubusercontent.com/Wiciaki/Dependencies/master/Download/";

        /// <summary>
        /// The minimum .NET Framework version
        /// </summary>
        private const int MinRecommendedDotNetVersion = 394802;

        /// <summary>
        /// Initializes static members of the <see cref="Program"/> class
        /// </summary>
        static Program()
        {
            var librariesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EloBuddy", "Addons", "Libraries");
            Directory.CreateDirectory(librariesPath);

            var libs = new List<string> { "SparkTech.SDK", "NLog", "MoreLinq", "JetBrains.Annotations" };

            Resources = libs.ConvertAll(name =>
            {
                var dll = name + ".dll";

                return new ResourceData { Name = name, CloudPath = BaseWebPath + dll, LocalPath = Path.Combine(librariesPath, dll) };
            });
        }

        /// <summary>
        /// The entry point for an application
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var console = false;

            if (!FrameworkUpdated())
            {
                AllocConsole();
                console = true;
                Console.Write("It's recommended to have .NET Framework 4.6.2 or newer installed - which was not detected on your system.\nProceeding anyway...\n\n");
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

            using (var client = new WebClient())
            {
                var assemblyName = typeof(Program).Assembly.GetName();
                var web = client.DownloadString(BaseWebPath + assemblyName.Name + ".txt");

                if (assemblyName.Version < new Version(web))
                {
                    File.WriteAllBytes(updater, Properties.Resources.Updater);
                    Process.Start(updater);
                    Environment.Exit(0);
                }
                
                string elobuddy;

                if (!new DirectoryInfo(dir).Name.Equals("elobuddy", StringComparison.OrdinalIgnoreCase) || (elobuddy = Directory.GetFiles(dir).Select(Path.GetFileName).SingleOrDefault(x => x.Equals("elobuddy.loader.exe", StringComparison.OrdinalIgnoreCase))) == null)
                {
                    if (!console)
                    {
                        AllocConsole();
                    }

                    Console.WriteLine("This file must be placed in EloBuddy installation folder!\nPlease move this file and try again.");
                    Console.Read();
                    return;
                }

                Process.Start(elobuddy);

                foreach (var file in Resources)
                {
                    Console.Write($@"Downloading {file.Name}...");
                    client.DownloadFile(file.CloudPath, file.LocalPath);
                    Console.WriteLine(@" done!");
                }
            }
        }

        /// <summary>
        /// Determines whether at least the recommended version of 
        /// </summary>
        /// <returns></returns>
        private static bool FrameworkUpdated()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
            return key != null && Convert.ToUInt32(key.GetValue("Release", 0)) > MinRecommendedDotNetVersion;
        }
    }
}