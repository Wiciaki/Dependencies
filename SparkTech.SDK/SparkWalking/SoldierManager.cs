namespace SparkTech.SDK.SparkWalking
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using SharpDX;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Executors;

    using Tracker = SparkTech.SDK.Utils.ObjectTracker<EloBuddy.Obj_AI_Minion>;

    [Trigger]
    public static class SoldierManager
    {
        private static readonly Dictionary<int, Tracker> Trackers;

        static SoldierManager()
        {
            var heroes = ObjectCache.GetNative<AIHeroClient>();

            Trackers = new Dictionary<int, Tracker>(heroes.Count);

            foreach (var id in heroes.Where(enemy => enemy.ChampionName == "Azir").Select(enemy => enemy.NetworkId))
            {
                Trackers.Add(id, new Tracker("azirsoldier", "azirw", id));
            }
        }

        public static Tracker GetTracker(AIHeroClient hero)
        {
            return GetTracker(hero.NetworkId);
        }

        public static Tracker GetTracker(int networkId)
        {
            return Trackers[networkId];
        }

        private static List<Obj_AI_Minion> FilterReady(List<Obj_AI_Minion> soldiers, Vector2 ownerPos)
        {
            return soldiers.FindAll(soldier => !soldier.IsMoving && soldier.Distance(ownerPos) <= 800F);
        }
    }
}