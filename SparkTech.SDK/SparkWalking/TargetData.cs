namespace SparkTech.SDK.SparkWalking
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;

    /// <summary>
    /// Provides informations about the target
    /// </summary>
    public struct TargetData
    {
        /// <summary>
        /// The <see cref="AttackableUnit"/> instance
        /// </summary>
        public readonly AttackableUnit Target;

        /// <summary>
        /// Indicates whether the player should wait for a better target to be found
        /// </summary>
        public readonly bool ShouldWait;

        /// <summary>
        /// Determines whether the current <see cref="TargetData"/> should be used
        /// </summary>
        public bool IsValid => this.Target != null || this.ShouldWait;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        /// <param name="shouldWait">Determines whether this instance should wait</param>
        public TargetData(bool shouldWait)
        {
            this.Target = null;

            this.ShouldWait = shouldWait;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        /// <param name="target">The target</param>
        public TargetData(AttackableUnit target)
        {
            this.Target = target;

            this.ShouldWait = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        /// <param name="orderedEnumerable">The sorted targets</param>
        public TargetData(IEnumerable<AttackableUnit> orderedEnumerable) : this(orderedEnumerable?.FirstOrDefault())
        {
            
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TargetData))
            {
                return false;
            }

            return this == (TargetData)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TargetData left, TargetData right)
        {
            if (left.ShouldWait != right.ShouldWait)
            {
                return false;
            }

            if (left.Target == null)
            {
                return right.Target == null;
            }

            if (right.Target == null)
            {
                return false;
            }

            return left.Target.NetworkId == right.Target.NetworkId;
        }

        public static bool operator !=(TargetData left, TargetData right)
        {
            return !(left == right);
        }
    }
}