namespace SparkTech.SDK.Executors
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Utils;

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
        /// Initializes static members of the <see cref="Bootstrap"/> class
        /// </summary>
        static Bootstrap()
        {
            HandleTrigger(Assembly = typeof(Bootstrap).Assembly);
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        /// <param name="bootstrapLinkType">The link type</param>
        public static void Handle(this string[] args, Type bootstrapLinkType)
        {
            Array.ForEach(args, Console.WriteLine);

            HandleTrigger(bootstrapLinkType.Assembly);
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
                            Logger.Error(ex.ToString());
                        }
                    }
                };
        }
    }
}