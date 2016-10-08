namespace SparkTech.SDK.EventData
{
    using System;

    using EloBuddy;

    /// <summary>
    /// The instance containing the AfterPlayerAttack event args
    /// </summary>
    public class AfterAttackEventArgs : EventArgs
    {
        /// <summary>
        /// The target
        /// </summary>
        public readonly AttackableUnit Target;

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterAttackEventArgs"/>
        /// </summary>
        /// <param name="target"></param>
        internal AfterAttackEventArgs(AttackableUnit target)
        {
            this.Target = target;
        }
    }
}