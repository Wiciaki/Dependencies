namespace SparkTech.SDK.Enumerations
{
    /// <summary>
    /// Represents informations regarding the current orbwalking mode
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// The orbwalker is performing a combo
        /// </summary>
        Combo = 1, 

        /// <summary>
        /// The orbwalker is clearing the minions
        /// </summary>
        LaneClear = 2, 

        /// <summary>
        /// The orbwalker is harassing
        /// </summary>
        Harass = 3, 

        /// <summary>
        /// The orbwalker finishes minions
        /// </summary>
        LastHit = 4, 

        /// <summary>
        /// The orbwalker freezes the lane
        /// </summary>
        Freeze = 5, 

        /// <summary>
        /// The orbwalker flees
        /// </summary>
        Flee = 6, 

        /// <summary>
        /// The orbwalker is slacking
        /// </summary>
        None = 0
    }
}