using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;

namespace SparkTech.Loader
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    internal static class Program
    {
        private struct Handlable
        {
            public string Name;

            public string CloudPath;

            public string LocalPath;
        }

        private static readonly List<Handlable> Files;

        private const string BaseWebPath = "https://raw.githubusercontent.com/Wiciaki/Dependencies/master/Download/";

        static Program()
        {
            var librariesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EloBuddy", "Addons", "Libraries");

            var libs = new List<string> { "SparkTech.SDK", "NLog", "MoreLinq" };
            Files = new List<Handlable>(libs.Count);

            foreach (var name in libs)
            {
                var dll = name + ".dll";

                Files.Add(new Handlable
                {
                    Name = name, CloudPath = BaseWebPath + dll, LocalPath = Path.Combine(librariesPath, dll)
                });
            }
        }

        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            
            Console.Write("Welcome! EloBuddy is starting soon...\n\n");

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
                    Console.WriteLine("This file must be placed in EloBuddy installation folder!\nPlease move this file and try again.");
                    Console.Read();
                    return;
                }

                foreach (var file in Files)
                {
                    Console.Write($@"Downloading {file.Name}...");
                    client.DownloadFile(file.CloudPath, file.LocalPath);
                    Console.WriteLine(@" done!");
                }
            }

            ExitTo(loader);
        }

        private static void ExitTo(string fileName)
        {
            Process.Start(fileName);
            Environment.Exit(0);
        }
    }
}