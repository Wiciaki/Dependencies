namespace SparkTech.SDK.Enumerations
{
    using System;

    /// <summary>
    /// Represents a set of object team flags
    /// </summary>
    [Flags]
    public enum ObjectTeam
    {
        /// <summary>
        /// The none flag
        /// </summary>
        None = 0,

        /// <summary>
        /// The enemy flag
        /// </summary>
        Enemy = 1 << 1,

        /// <summary>
        /// The ally flag
        /// </summary>
        Ally = 1 << 2,

        /// <summary>
        /// The neutral flag
        /// </summary>
        Neutral = 1 << 3,

        /// <summary>
        /// The unknown flag
        /// </summary>
        Unknown = 1 << 4
    }
}