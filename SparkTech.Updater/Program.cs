namespace SparkTech.Updater
{
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    internal static class Program
    {
        private static void Main(string[] args)
        {
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