namespace SparkTech.Addon
{
    using SparkTech.SDK.Executors;

    /// <summary>
    /// The bootsrap link to initialize an assembly
    /// </summary>
    internal static class BootstrapLink
    {
        /// <summary>
        /// The entry point for an application
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        private static void Main(string[] args)
        {
            args.Handle(typeof(BootstrapLink));
        }
    }
}