using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Updater
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Console.WriteLine("Please wait...");

            const string loaderName = "SparkTech.Loader.exe";
            var file = Path.Combine(Directory.GetCurrentDirectory(), loaderName);

            using (var client = new WebClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/Wiciaki/Dependencies/master/Download/" + loaderName, file);
            }

            Process.Start(file);
            Environment.Exit(0);
        }
    }
}