namespace SparkTech.SDK.Web
{
    using System;

    /// <summary>
    /// Provides additional utility related to the web client
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Determines whether the input link is a valid link as well as creates an <see cref="Uri"/> out of it
        /// </summary>
        /// <param name="link">The link to be inspected</param>
        /// <param name="uri">The output <see cref="Uri"/></param>
        /// <returns></returns>
        public static bool IsLinkValid(string link, out Uri uri)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                uri = null;
                return false;
            }

            return Uri.TryCreate(link, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Determines whether the input link is a valid link
        /// </summary>
        /// <param name="link">The link to be inspected</param>
        /// <returns></returns>
        public static bool IsLinkValid(string link)
        {
            Uri dump;
            return IsLinkValid(link, out dump);
        }

        /// <summary>
        /// Determines whether the specified link is a valid raw assembly info link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static bool AssemblyInfoValidation(string link)
        {
            link = link.ToLower();

            return link.Contains("raw.githubusercontent.com") && link.Contains("assemblyinfo.cs");
        }
    }
}