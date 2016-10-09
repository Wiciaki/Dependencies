namespace SparkTech.Addon
{
    using System;
    using System.Drawing;
    using System.Reflection;

    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Web;

    /// <summary>
    /// The main class
    /// </summary>
    [Trigger]
    public static class Addon
    {
        /// <summary>
        /// The main menu instance of the addon
        /// </summary>
        private static readonly Menu Menu;

        /// <summary>
        /// The mode menu instance
        /// </summary>
        private static readonly Menu Combo, Harass;

        /// <summary>
        /// Initializes static members of the <see cref="Addon"/> class
        /// </summary>
        static Addon()
        {
            // Here is the right place to put your code, this is safe context.
            // This code executes after the game has started.

            // TODO: Fill it with your data :)
            const string MyAssemblyInfoLink = "https://raw.githubusercontent.com/.../.../master/.../Properties/AssemblyInfo.cs";

            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var name = assemblyName.Name;

            Chat.Print($"Loading {name}..");

            #region Menu Creator

            var codeName = "st.addon";

            Menu = MainMenu.AddMenu(name, codeName);
            {
                codeName += ".";

                Combo = Menu.AddSubMenu("Combo", codeName + "combo");
                {
                    Combo.Add(codeName + "combo.q", new CheckBox("Q usage"));
                }

                Harass = Menu.AddSubMenu("Harass", codeName + "harass");
                {

                }
            }

            #endregion

            #region Event Subscriptions

            var updater = new AssemblyInfoUpdater(MyAssemblyInfoLink, assemblyName.Version, name);

            updater.CheckPerformed += OnCheckPerformed;
            Game.OnTick += OnTick;
            Drawing.OnDraw += OnDraw;

            #endregion
        }

        /// <summary>
        /// The <see cref="E:CheckPerformed"/> event handler
        /// <para>This is used for handling the update check</para>
        /// </summary>
        /// <param name="args">The event data</param>
        private static void OnCheckPerformed(CheckPerformedEventArgs args)
        {
            // if (args.Success && !args.IsUpdated)
            // {
            //     Chat.Print("This assembly is outdated! Please update it in the loader!");
            // }

            args.Notify();
        }

        /// <summary>
        /// The <see cref="E:OnTick"/> action, fired every game tick
        /// </summary>
        /// <param name="args">The empty <see cref="EventArgs"/> instance</param>
        private static void OnTick(EventArgs args)
        {
            if (Combo["st.addon.combo.q"].Cast<CheckBox>().CurrentValue)
            {
                
            }
        }

        /// <summary>
        /// The <see cref="E:OnDraw"/> action, fired every frame
        /// </summary>
        /// <param name="args">The empty <see cref="EventArgs"/> instance</param>
        private static void OnDraw(EventArgs args)
        {
            var position = Drawing.WorldToScreen(ObjectCache.Player.Position);

            Drawing.DrawText(position.X, position.Y, Color.Gold, "Template initialized");
        }
    }
}