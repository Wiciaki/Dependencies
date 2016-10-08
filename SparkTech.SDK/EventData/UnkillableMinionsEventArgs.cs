namespace SparkTech.SDK.EventData
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;

    /// <summary>
    /// The UnkillableMinions event args
    /// </summary>
    public class UnkillableMinionsEventArgs : EventArgs
    {
        /// <summary>
        /// The minions which can't be killed using nothing but auto-attacks
        /// </summary>
        public readonly List<Obj_AI_Minion> Minions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnkillableMinionsEventArgs"/> class
        /// </summary>
        /// <param name="minions"></param>
        public UnkillableMinionsEventArgs(List<Obj_AI_Minion> minions)
        {
            this.Minions = minions;
        }
    }
}