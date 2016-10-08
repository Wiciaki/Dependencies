namespace SparkTech.SDK.Utils
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using EloBuddy;
    using EloBuddy.SDK;

    using SparkTech.SDK.Enumerations;

    public static class Minion
    {
        public static bool IsJungleBuff(this Obj_AI_Minion minion)
        {
            var @base = minion.CharData.BaseSkinName;
            return @base == "SRU_Blue" || @base == "SRU_Red";
        }

        public static bool IsMinion(this Obj_AI_Minion minion)
        {
            return minion.DetermineType().IsMinion();
        }

        public static bool IsMinion(this AIMinionType type)
        {
            return type == AIMinionType.Normal || type == AIMinionType.Siege || type == AIMinionType.Super;
        }

        public static bool IsWard(this Obj_AI_Minion minion)
        {
            return minion.DetermineType() == AIMinionType.Ward;
        }

        public static bool IsJungle(this Obj_AI_Minion minion)
        {
            return minion.DetermineType().IsJungle();
        }

        public static bool IsJungle(this AIMinionType type)
        {
            return type == AIMinionType.JungleSmall || type == AIMinionType.JungleMedium || type == AIMinionType.JungleLarge;
        }

        public static bool IsPet(this Obj_AI_Minion minion)
        {
            return minion.DetermineType() == AIMinionType.Pet;
        }

        public static AIMinionType DetermineType(this Obj_AI_Minion minion)
        {
            if (minion.IsValid())
            {
                var @base = minion.CharData.BaseSkinName;

                if (NormalMinionList.Exists(@base.Equals))
                {
                    return AIMinionType.Normal;
                }

                if (SiegeMinionList.Exists(@base.Equals))
                {
                    return AIMinionType.Siege;
                }

                if (SuperMinionList.Exists(@base.Equals))
                {
                    return AIMinionType.Super;
                }

                @base = @base.ToLower();

                if (@base.Contains("ward") || @base.Contains("trinket"))
                {
                    return AIMinionType.Ward;
                }

                if (PetList.Exists(@base.Equals))
                {
                    return AIMinionType.Pet;
                }

                var name = minion.Name;

                if (SmallNameRegex.Exists(regex => Regex.IsMatch(name, regex)))
                {
                    return AIMinionType.JungleSmall;
                }

                if (LargeNameRegex.Exists(regex => Regex.IsMatch(name, regex)))
                {
                    return AIMinionType.JungleMedium;
                }

                if (LegendaryNameRegex.Exists(regex => Regex.IsMatch(name, regex)))
                {
                    return AIMinionType.JungleLarge;
                }
            }

            return AIMinionType.Unknown;
        }

        #region Database
        
        private static readonly List<string> NormalMinionList = new List<string>
                                                                    {
                                                                        "SRU_ChaosMinionMelee", "SRU_ChaosMinionRanged",
                                                                        "SRU_OrderMinionMelee", "SRU_OrderMinionRanged",
                                                                        "HA_ChaosMinionMelee", "HA_ChaosMinionRanged",
                                                                        "HA_OrderMinionMelee", "HA_OrderMinionRanged"
                                                                    };

        private static readonly List<string> PetList = new List<string>
                                                           {
                                                               "annietibbers", "elisespiderling", "heimertyellow",
                                                               "heimertblue", "malzaharvoidling", "shacobox",
                                                               "yorickspectralghoul", "yorickdecayedghoul",
                                                               "yorickravenousghoul", "zyrathornplant",
                                                               "zyragraspingplant", "illaoitentacle", /* TODO verify */
                                                               "leblanc", "shaco", "monkeyking"
                                                           };

        private static readonly List<string> SiegeMinionList = new List<string>
                                                                   {
                                                                       "SRU_ChaosMinionSiege", "SRU_OrderMinionSiege",
                                                                       "HA_ChaosMinionSiege", "HA_OrderMinionSiege"
                                                                   };

        private static readonly List<string> SuperMinionList = new List<string>
                                                                   {
                                                                       "SRU_ChaosMinionSuper", "SRU_OrderMinionSuper",
                                                                       "HA_ChaosMinionSuper", "HA_OrderMinionSuper"
                                                                   };

        private static readonly List<string> SmallNameRegex = new List<string> { "SRU_[a-zA-Z](.*?)Mini", "Sru_Crab" };

        private static readonly List<string> LargeNameRegex = new List<string>
            {
                "SRU_Murkwolf[0-9.]{1,}", "SRU_Gromp", "SRU_Blue[0-9.]{1,}",
                "SRU_Razorbeak[0-9.]{1,}", "SRU_Red[0-9.]{1,}",
                "SRU_Krug[0-9]{1,}"
            };

        private static readonly List<string> LegendaryNameRegex = new List<string> { "SRU_Dragon", "SRU_Baron", "SRU_RiftHerald" };

        #endregion
    }
}