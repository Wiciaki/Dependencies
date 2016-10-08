namespace SparkTech.SDK.Enumerations
{
    using System.ComponentModel;

    /// <summary>
    /// Represents the modes humanizer can use
    /// </summary>
    public enum HumanizerMode
    {
        /// <summary>
        /// The disabled mode
        /// </summary>
        Off = 1, 

        /// <summary>
        /// The challenger mode
        /// </summary>
        [Description("20-70")]
        Challenger = 2, 

        /// <summary>
        /// The normal mode
        /// </summary>
        [Description("70-250")]
        Normal = 0, 

        /// <summary>
        /// The bronze mode
        /// </summary>
        [Description("250-1000")]
        Bronze = 3
    }
}