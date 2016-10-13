namespace SparkTech.SDK.Web
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    using SparkTech.SDK.Utils;

    public class SparkTechUpdater : Updater
    {
        private static string data;

        private static bool downloading;

        private readonly string AssemblyName;

        private readonly Version Version;

        private readonly Regex Regex;

        public SparkTechUpdater(Version version, string assemblyName, string internalName = null) : base(Variables.RawVariablesPath)
        {
            this.Version = version;

            this.AssemblyName = assemblyName;

            var pattern = "Version" + (internalName ?? assemblyName) + @" = ""(\d.\d.\d.\d)""";

            this.Regex = new Regex(pattern, RegexOptions.CultureInvariant);

            if (this.IsLinkValid)
            {
                this.PerformCheck();
            }
        }

        private async void PerformCheck()
        {
            if (downloading)
            {
                TickOperation.ExecuteOnNextTick(this.PerformCheck);
                return;
            }

            if (data == null)
            {
                downloading = true;

                using (var client = new WebClient())
                {
                    data = await client.DownloadStringTaskAsync(this.Link).ConfigureAwait(false);
                }

                downloading = false;
            }

            TickOperation.ExecuteOnNextTick(delegate
            {
                var match = this.Regex.Match(data);
                var gitVersion = match.Success ? new Version(match.Groups[1].Value) : null;

                this.RaiseEvent(gitVersion, this.Version, this.AssemblyName);
            });
        }
    }
}