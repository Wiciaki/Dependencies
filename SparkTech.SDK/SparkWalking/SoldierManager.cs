namespace SparkTech.SDK.SparkWalking
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using SharpDX;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;

    using Tracker = SparkTech.SDK.Utils.ObjectTracker<EloBuddy.Obj_AI_Minion>;

    /// <summary>
    /// The management class for Azirs' soldiers
    /// </summary>
    [Trigger]
    public static class SoldierManager
    {
        /// <summary>
        /// Contains the trackers
        /// </summary>
        private static readonly Dictionary<int, Tracker> Trackers;

        /// <summary>
        /// Initializes static members of the <see cref="SoldierManager"/> class
        /// </summary>
        static SoldierManager()
        {
            var heroes = ObjectCache.GetNative<AIHeroClient>();

            Trackers = new Dictionary<int, Tracker>(heroes.Count);

            foreach (var id in heroes.Where(enemy => enemy.ChampionName == "Azir").Select(enemy => enemy.NetworkId))
            {
                Trackers.Add(id, new Tracker("azirsoldier", "azirw", id));
            }
        }

        /// <summary>
        /// Gets a tracker
        /// </summary>
        /// <param name="hero">The Azir</param>
        /// <returns></returns>
        public static Tracker GetTracker(AIHeroClient hero)
        {
            return GetTracker(hero.NetworkId);
        }

        /// <summary>
        /// Gets a tracker
        /// </summary>
        /// <param name="networkId">The Azir's NetworkId</param>
        /// <returns></returns>
        public static Tracker GetTracker(int networkId)
        {
            return Trackers[networkId];
        }

        /// <summary>
        /// Gets a list of soldiers that can reach the target
        /// </summary>
        /// <param name="source">The source hero. It has to be Azir</param>
        /// <param name="target">The target hero</param>
        /// <returns></returns>
        public static List<Obj_AI_Minion> GetSoldiers(this AIHeroClient source, Obj_AI_Base target)
        {
            return GetSoldiers(target, GetTracker(source), source.ServerPosition);
        }

        /// <summary>
        /// Gets a list of soldiers that can reach the target.
        /// <para>Both tracker and position must be of the same owner for this method to work correctly</para>
        /// </summary>
        /// <param name="target">The enemy you want to get soldiers at</param>
        /// <param name="tracker">The tracker of Azir</param>
        /// <param name="ownerPos">The position of Azir</param>
        /// <returns></returns>
        public static List<Obj_AI_Minion> GetSoldiers(Obj_AI_Base target, Tracker tracker, Vector3 ownerPos)
        {
            // Assumptions for the player to attack a target using soldiers:
            // Player is Azir
            // The target can be attacked
            // Target is a hero, a lane or a jungle minion
            // Target is no more than 1000 units from player
            // Target is no more than 350 units from the soldier
            // Soldier is no more than 800 units from player
            // Soldier isn't moving

            if (!target.IsValidTarget() || target.Distance(ownerPos) > 1000F)
            {
                return MiscallenousCache.GetEmptyList<Obj_AI_Minion>();
            }

            var minion = target as Obj_AI_Minion;

            if (minion != null)
            {
                var type = minion.DetermineType();

                if (type == AIMinionType.Ward || type == AIMinionType.Unknown)
                {
                    return MiscallenousCache.GetEmptyList<Obj_AI_Minion>();
                }
            }
            else
            {
                if (!(target is AIHeroClient))
                {
                    return MiscallenousCache.GetEmptyList<Obj_AI_Minion>();
                }
            }

            return tracker.Items.FindAll(soldier => !soldier.IsMoving && soldier.Distance(ownerPos) <= 800F);
        }
    }
}