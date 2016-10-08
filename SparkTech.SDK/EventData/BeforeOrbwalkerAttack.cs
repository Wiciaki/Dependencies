namespace SparkTech.SDK.EventData
{
    using System;

    using EloBuddy;

    /// <summary>
    /// Contains the pre-attack event data
    /// </summary>
    public class BeforeOrbwalkerAttack : EventArgs
    {
        /// <summary>
        /// The target
        /// </summary>
        public readonly AttackableUnit Target;

        /// <summary>
        /// Gets or sets whether the attack should be cancelled
        /// </summary>
        public bool CancelAttack;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeOrbwalkerAttack"/> class
        /// </summary>
        /// <param name="target">The target</param>
        internal BeforeOrbwalkerAttack(AttackableUnit target)
        {
            this.Target = target;
        }
    }
}