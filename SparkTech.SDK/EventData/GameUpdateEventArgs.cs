namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Enumerations;

    /// <summary>
    /// The instance containing the GameUpdate event args
    /// </summary>
    public class GameUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The current orbwalking <see cref="Mode"/>
        /// </summary>
        public readonly Mode OrbwalkingMode;

        /// <summary>
        /// The current <see cref="Orbwalker"/>
        /// </summary>
        public readonly Orbwalker Orbwalker;

        internal GameUpdateEventArgs(Orbwalker orbwalker, Mode mode)
        {
            this.Orbwalker = orbwalker;

            this.OrbwalkingMode = mode;
        }
    }
}