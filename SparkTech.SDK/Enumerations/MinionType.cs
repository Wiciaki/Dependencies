namespace SparkTech.SDK.Enumerations
{
    using System;

    /// <summary>
    /// The minion type flags
    /// </summary>
    [Flags]
    public enum MinionType
    {
        /// <summary>
        /// The unknown flag
        /// </summary>
        None = 0, 

        /// <summary>
        /// The real minion flag
        /// </summary>
        Minion = 1 << 1, 

        /// <summary>
        /// The ward flag
        /// </summary>
        Ward = 1 << 2, 

        /// <summary>
        /// The pet flag
        /// </summary>
        Pet = 1 << 3, 

        /// <summary>
        /// The jungle flag
        /// </summary>
        Jungle = 1 << 4, 

        /// <summary>
        /// The other flag
        /// </summary>
        Other = 1 << 5
    }
}