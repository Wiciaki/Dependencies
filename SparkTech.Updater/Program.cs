namespace SparkTech.Updater
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length > 0)
            {
                File.Delete(args[0]);
            }

            const string LoaderName = "SparkTech.Loader.exe";

            var file = Path.Combine(Directory.GetCurrentDirectory(), LoaderName);

            using (var client = new WebClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/Wiciaki/Dependencies/master/Download/" + LoaderName, file);
            }

            Process.Start(file);
        }
    }
}