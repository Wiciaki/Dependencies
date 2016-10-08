namespace SparkTech.SDK.Targeting
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;

    using Targets = System.Collections.Generic.List<EloBuddy.AIHeroClient>;

    public static class TargetAcquire
    {
        public static List<Weight> Weights = new List<Weight>();

        private static Targets backup = new Targets();

        private static IEnumerable<AIHeroClient> Targets
        {
            get
            {
                if (updated)
                {
                    return backup;
                }

                var heroes = ObjectCache.Get<AIHeroClient>(ObjectTeam.Enemy);
                var dict = new Dictionary<AIHeroClient, int>(heroes.Count);

                foreach (var hero in heroes)
                {
                    dict.Add(hero, 0);
                }

                foreach (var weight in Weights)
                {
                    
                }

                updated = true;
                return backup = dict.Keys.ToList();
            }
        }

        private static bool updated;

        static TargetAcquire()
        {
            Game.OnUpdate += delegate
            {
                updated = false;
            };
        }

        public static Targets GetTargets()
        {
            return Targets.ToList();
        }
    }
}