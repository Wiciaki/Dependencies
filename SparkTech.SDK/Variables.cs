namespace SparkTech.SDK
{
    using System;
    using System.Reflection;

    using EloBuddy.Sandbox;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Web;

    /// <summary>
    /// The delegate used for passing argument-less Boolean pointers
    /// </summary>
    /// <returns></returns>
    public delegate bool Predicate();

    /// <summary>
    /// The main event delegate used for handling most of the event data instances
    /// </summary>
    /// <typeparam name="TEventArgs">The destination event arguments</typeparam>
    /// <param name="args">The event data</param>
    public delegate void EventDataHandler<in TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;

    /// <summary>
    /// The variable storage and menu initializer
    /// </summary>
    [Trigger]
    public static class Variables
    {
        /// <summary>
        /// The version for the current assembly
        /// </summary>
        public const string VersionSDK = "0.1.1.3";

        /// <summary>
        /// The version for the <see cref="E:AllyPingSpammer"/>
        /// </summary>
        public const string VersionAllyPingSpammer = "1.2.1.2";

        /// <summary>
        /// The path to web version of the current class
        /// </summary>
        public const string RawVariablesPath = "https://raw.githubusercontent.com/Wiciaki/Dependencies/master/SparkTech.SDK/Variables.cs";

        /// <summary>
        /// Determines whether this run is a first one in the specified environment
        /// </summary>
        public static readonly bool FirstRun;

        /// <summary>
        /// Gets the currently executing assembly
        /// </summary>
        public static Assembly Assembly => Bootstrap.Assembly;

        /// <summary>
        /// The SDK menu
        /// </summary>
        public static readonly Menu SDKMenu;

        /// <summary>
        /// Initializes static members of the <see cref="Variables"/> class
        /// </summary>
        static Variables()
        {
            SDKMenu = MainMenu.AddMenu("SparkTech.SDK", "st.sdk", "SparkTech Software Development Kit");

            #region FirstInit
            {
                var first = new CheckBox("ERROR") { IsVisible = false };
                SDKMenu.Add("st.sdk.firstrun", first);
                FirstRun = first.CurrentValue;
                first.CurrentValue = false;
            }
            #endregion

            #region Version
            {
                var version = SDKMenu.AddSubMenu("Version", "st.sdk.version");
                var allow = new CheckBox("Allow update checks");
                version.Add("st.sdk.info.version.check", allow);
                var sdkVerion = new Label(allow.CurrentValue ? "Version - Checking failure!" : "Update checks disabled");
                version.Add("st.sdk.info.version", sdkVerion);

                if (allow.CurrentValue)
                {
                    var assemblyName = Assembly.GetName();

                    new SparkTechUpdater(assemblyName.Version, assemblyName.Name, "SDK").CheckPerformed += args =>
                    {
                        if (!args.Success)
                        {
                            return;
                        }

                        sdkVerion.DisplayName = args.IsUpdated
                                                  ? "Your copy of SparkTech.SDK is up to date"
                                                  : "A new update is available!";

                        args.Notify();
                    };
                }
            }
            #endregion

            #region Info
            {
                var info = SDKMenu.AddSubMenu("About", "st.sdk.info");
                info.AddLabel($"Welcome, \"{SandboxConfig.Username}\" :)");
                info.AddLabel($"License type: {(SandboxConfig.IsBuddy ? "Buddy" : "Pleb")}");
                info.AddSeparator();
                info.AddGroupLabel("Please report any bugs or suggestions at:");
                info.AddLabel("Skype: \"wiktorsharp\"");
                info.AddLabel("Discord: @spark");
            }
            #endregion

            
        }
    }
}