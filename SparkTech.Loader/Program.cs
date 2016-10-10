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
    using System.Threading;

    using IWshRuntimeLibrary;

    using File = System.IO.File;

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
        private struct ResourceData
        {
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
        /// Initializes static members of the <see cref="Program"/> class
        /// </summary>
        static Program()
        {
            var librariesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EloBuddy",
                "Addons",
                "Libraries");

            Directory.CreateDirectory(librariesPath);

            var libs = new List<string> { "SparkTech.SDK", "MoreLinq", "JetBrains.Annotations" };

            Resources = libs.ConvertAll(name =>
                    {
                        name += ".dll";

                        return new ResourceData { CloudPath = BaseWebPath + name, LocalPath = Path.Combine(librariesPath, name) };
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
            
            var dir = Directory.GetCurrentDirectory();

            var updater = Path.Combine(dir, "SparkTech.Updater.exe");
            Delete(updater);
            Delete(Path.Combine(dir, "Updater.exe"));

            var assembly = typeof(Program).Assembly;
            var assemblyName = assembly.GetName();
            var name = assemblyName.Name;

            using (var client = new WebClient())
            {
                var web = client.DownloadString(BaseWebPath + name + ".txt");

                if (assemblyName.Version < new Version(web))
                {
                    File.WriteAllBytes(updater, Properties.Resources.SparkTech_Updater);
                    Process.Start(updater);
                    return;
                }

                string elobuddy;

                if ((elobuddy = Directory.GetFiles(dir).Select(Path.GetFileName).SingleOrDefault(x => x.Equals("elobuddy.loader.exe", StringComparison.OrdinalIgnoreCase))) == null)
                {
                    AllocConsole();
                    Console.Write("This file must be placed in the same folder as EloBuddy.Loader.exe!\nPlease move this file and try again.\n");
                    Console.Read();
                    return;
                }

                if (!File.Exists(Path.Combine(dir, "noshortcut")))
                {
                    var shortcut = (IWshShortcut)new WshShell().CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SparkTech.lnk"));
                    shortcut.TargetPath = Path.Combine(dir, name + ".exe");
                    shortcut.WorkingDirectory = dir;
                    shortcut.Save();
                }

                Process.Start(elobuddy);

                Thread.Sleep(3000);

                foreach (var file in Resources)
                {
                    client.DownloadFile(file.CloudPath, file.LocalPath);
                }
            }
        }

        private static void Delete(string path)
        {
            var file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
            }
        }
    }
}