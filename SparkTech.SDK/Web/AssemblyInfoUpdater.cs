namespace SparkTech.SDK.Web
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    /// <summary>
    /// <para>A child of the <see cref="Updater"/> class allowing to check for an update based on the AssemblyInfo file</para>
    /// </summary>
    public class AssemblyInfoUpdater : Updater
    {
        /// <summary>
        /// The regular expression
        /// </summary>
        private static readonly Regex Regex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoUpdater"/> class
        /// </summary>
        /// <param name="fullRawAssemblyInfoLink">The link to the assemblyinfo.cs file that can be found in your github. This has to be a raw link</param>
        /// <param name="localVersion">The local version of the assembly to compare</param>
        /// <param name="assemblyName">The name of the assembly</param>
        public AssemblyInfoUpdater(string fullRawAssemblyInfoLink, Version localVersion, string assemblyName) : base(fullRawAssemblyInfoLink)
        {
            if (this.IsLinkValid && Utility.AssemblyInfoValidation(this.Path))
            {
                this.UpdateCheck(localVersion, assemblyName);
            }
        }

        /// <summary>
        /// Performs an update check
        /// </summary>
        /// <param name="localVersion">The local version of the assembly</param>
        /// <param name="assemblyName">The assembly name</param>
        private async void UpdateCheck(Version localVersion, string assemblyName)
        {
            string data;

            using (var client = new WebClient())
            {
                data = await client.DownloadStringTaskAsync(this.Link).ConfigureAwait(false);
            }

            var match = Regex.Match(data);
            var gitVersion = match.Success ? new Version(match.Groups[1].Value) : null;

            this.RaiseEvent(gitVersion, localVersion, assemblyName);
        }
    }
}