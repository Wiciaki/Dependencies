namespace SparkTech.SDK.Executors
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using EloBuddy.SDK.Events;

    using NLog;

    /// <summary>
    /// The component initializer
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// The current assembly
        /// </summary>
        internal static readonly Assembly Assembly;

        /// <summary>
        /// The logger for the current type
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes static members of the <see cref="Bootstrap"/> class
        /// </summary>
        static Bootstrap()
        {
            HandleTrigger(Assembly = Assembly.GetAssembly(typeof(Bootstrap)));
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        /// <param name="bootstrapLinkType">The link type</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Handle(this string[] args, Type bootstrapLinkType)
        {
            Array.ForEach(args, Console.WriteLine);

            HandleTrigger(Assembly.GetAssembly(bootstrapLinkType));
        }

        /// <summary>
        /// Invokes .cctors of types with the <see cref="TriggerAttribute"/> in the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to have its types invoked</param>
        private static void HandleTrigger(Assembly assembly)
        {
            Loading.OnLoadingComplete += delegate
                {
                    foreach (var type in assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(TriggerAttribute), false).Length > 0).OrderBy(type => type.Name))
                    {
                        try
                        {
                            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                        }
                        catch (TypeInitializationException ex)
                        {
                            Logger.Fatal(ex);
                        }
                    }
                };
        }
    }
}