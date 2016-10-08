/*
namespace SparkTech.SDK.SparkWalking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;
    using SparkTech.SDK.Web;

    using DrawingClass = EloBuddy.Drawing;
    using Prediction = EloBuddy.SDK.Prediction.Position;

    /// <summary>
    /// An alternative to the <see cref="EloBuddy.SDK.Orbwalker"/> class.
    /// </summary>
    [Trigger]
    public class SparkWalker : Executable
    {
        #region Attackable Objects

        /// <summary>
        /// The <see cref="ObjectInfo"/> nested class
        /// </summary>
        private class ObjectInfo
        {
            /// <summary>
            /// The display name of the object
            /// </summary>
            internal readonly string DisplayName;

            /// <summary>
            /// The indication whether the item should be added to menu
            /// </summary>
            internal readonly bool AddToMenu;

            /// <summary>
            /// The indication whether to enable the menu item by default
            /// </summary>
            internal readonly bool AttackByDefault;

            /// <summary>
            /// The <see cref="Predicate"/> determining whether to attack
            /// </summary>
            internal readonly Predicate Attack;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectInfo"/> nested class
            /// </summary>
            /// <param name="attackByDef">Attack by default</param>
            /// <param name="displayName">Display name</param>
            internal ObjectInfo(string displayName, bool attackByDef = true)
            {
                displayName = displayName.ToMenuUse();

                this.AddToMenu = true;

                this.AttackByDefault = attackByDef;

                this.Attack = () => Menu["targeting"]["objects"][displayName];

                this.DisplayName = displayName;
            }
        }

        /// <summary>
        /// The <see cref="Dictionary{TKey,TValue}"/> holding the <see cref="ObjectInfo"/> data
        /// </summary>
        private static readonly Dictionary<string, ObjectInfo> AttackDictionary = new Dictionary<string, ObjectInfo>(StringComparer.OrdinalIgnoreCase)
                                                                                      {
                                                                                          {
                                                                                              "zyrathornplant",
                                                                                              new ObjectInfo("Zyra's thorn plant")
                                                                                          },
                                                                                          {
                                                                                              "zyragraspingplant",
                                                                                              new ObjectInfo("Zyra's grasping plant")
                                                                                          },
                                                                                          {
                                                                                              "illaoitentacle", // TODO verify
                                                                                              new ObjectInfo("Illaoi's tentacle")
                                                                                          },
                                                                                          {
                                                                                              "shacobox",
                                                                                              new ObjectInfo("Shaco's box")
                                                                                          },
                                                                                          {
                                                                                              "yorickdecayedghoul",
                                                                                              new ObjectInfo("Yorick's decayed ghoul")
                                                                                          },
                                                                                          {
                                                                                              "yorickravenousghoul",
                                                                                              new ObjectInfo("Yorick's ravenous ghoul")
                                                                                          },
                                                                                          {
                                                                                              "yorickspectralghoul",
                                                                                              new ObjectInfo("Yorick's spectral ghoul")
                                                                                          },
                                                                                          {
                                                                                              "heimertyellow",
                                                                                              new ObjectInfo("Heimer's yellow turret")
                                                                                          },
                                                                                          {
                                                                                              "heimertblue",
                                                                                              new ObjectInfo("Heimer's blue turret")
                                                                                          },
                                                                                          {
                                                                                              "malzaharvoidling",
                                                                                              new ObjectInfo("Malzahar's voidling")
                                                                                          },
                                                                                          {
                                                                                              "teemomushroom",
                                                                                              new ObjectInfo("Teemo's mushroom")
                                                                                          },
                                                                                          {
                                                                                              "elisespiderling",
                                                                                              new ObjectInfo("Elise's spiderling")
                                                                                          },
                                                                                          {
                                                                                              "annietibbers",
                                                                                              new ObjectInfo("Annie's tibbers", false)
                                                                                          },
                                                                                          {
                                                                                              "gangplankbarrel",
                                                                                              new ObjectInfo("Gangplank's barrel", false)
                                                                                          },
                                                                                          {
                                                                                              "leblanc",
                                                                                              new ObjectInfo("LeBlanc's clone", false)
                                                                                          },
                                                                                          {
                                                                                              "shaco",
                                                                                              new ObjectInfo("Shaco's clone", false)
                                                                                          },
                                                                                          {
                                                                                              "monkeyking",
                                                                                              new ObjectInfo("Wukong's clone", false)
                                                                                          }
                                                                                      };

        #endregion

        #region Configuration

        /// <summary>
        /// Contains the mode's configuration data
        /// </summary>
        private class ModeConfig
        {
            /// <summary>
            /// The key for mode to be activated
            /// </summary>
            internal readonly Key Key;

            /// <summary>
            /// An array of <see cref="UnitType"/> the mode should be enabled by default for
            /// </summary>
            internal readonly UnitType[] Units;

            /// <summary>
            /// Initializes a new instance of the <see cref="ModeConfig"/> class
            /// </summary>
            /// <param name="key">The key for mode to be activated</param>
            /// <param name="unitsEnabled">An array of <see cref="UnitType"/> the mode should be enabled by default for</param>
            internal ModeConfig(Key key, params UnitType[] unitsEnabled)
            {
                this.Key = key;

                this.Units = new UnitType[EnumCache<UnitType>.Count];

                Array.Copy(unitsEnabled, this.Units, unitsEnabled.Length);
            }
        }

        /// <summary>
        /// Contains the configuration for each of the modes
        /// </summary>
        private static readonly Dictionary<Mode, ModeConfig> ModeConfiguration = new Dictionary<Mode, ModeConfig>
                                                                                     {
                                                                                         {
                                                                                             Mode.Combo,
                                                                                             new ModeConfig(
                                                                                             Key.Space,
                                                                                             UnitType.Champion)
                                                                                         },
                                                                                         {
                                                                                             Mode.LaneClear,
                                                                                             new ModeConfig(
                                                                                             Key.V,
                                                                                             UnitType.LaneMinion,
                                                                                             UnitType.Structure,
                                                                                             UnitType.Object,
                                                                                             UnitType.Champion,
                                                                                             UnitType.LaneClearMinion,
                                                                                             UnitType.JungleMinion)
                                                                                         },
                                                                                         {
                                                                                             Mode.Harass,
                                                                                             new ModeConfig(
                                                                                             Key.C,
                                                                                             UnitType.LaneMinion,
                                                                                             UnitType.Champion)
                                                                                         },
                                                                                         {
                                                                                             Mode.Freeze,
                                                                                             new ModeConfig(
                                                                                             Key.A,
                                                                                             UnitType.LaneMinion)
                                                                                         },
                                                                                         {
                                                                                             Mode.LastHit,
                                                                                             new ModeConfig(
                                                                                             Key.X,
                                                                                             UnitType.LaneMinion)
                                                                                         },
                                                                                         {
                                                                                             Mode.Flee,
                                                                                             new ModeConfig(Key.Z)
                                                                                         }
                                                                                     };

        #endregion

        #region Orders

        /// <summary>
        /// Gets the attack order
        /// </summary>
        protected virtual GameObjectOrder AttackOrder => GameObjectOrder.AttackUnit;

        /// <summary>
        /// Gets the move order
        /// </summary>
        protected virtual GameObjectOrder MoveOrder => GameObjectOrder.MoveTo;

        /// <summary>
        /// Gets the stop order
        /// </summary>
        protected virtual GameObjectOrder StopOrder => Menu["misc"]["stop_order"].GetValue<MenuList<GameObjectOrder>>().SelectedValue;

        #endregion

        #region Static Variables

        /// <summary>
        /// Gets the current tick count
        /// </summary>
        protected virtual float TickCount => Game.Time * 1000f;

        /// <summary>
        /// A list of buff names getting which resets the auto attack timer
        /// </summary>
        private static readonly List<string> AttackResettingBuffs = new List<string> { "poppypassivebuff", "sonapassiveready" };

        /// <summary>
        /// The <see cref="System.Random"/> instance
        /// </summary>
        protected static readonly Random Random = new Random();

        /// <summary>
        /// The orbwalkEloBuddy.SDK.Menu.Menure.UI.IMenu.SDKMenu"/> instance
        /// </summary>
        protected static readonly Menu SDKMenu = new SDKMenu("st_orb", "SparkWalker [EARLY]");

        /// <summary>
        /// The names of the priorities
        /// </summary>
        private static readonly List<string> Priorities;

        /// <summary>
        /// Gets the extra holdzone radius
        /// </summary>
        public static int ExtraHoldZone => Menu["misc"]["extra_holdzone"];

        /// <summary>
        /// Gets the current farm delay
        /// </summary>
        public static int FarmDelay => Menu["misc"]["farmdelay"];

        #endregion

        #region Variables

        /// <summary>
        /// Gets or sets the <see cref="E:UnkillableMinions"/> scan range for this instance
        /// </summary>
        public Predicate<Obj_AI_Minion> Scanner;

        /// <summary>
        /// The cached <see cref="Vector3"/> position of the <see cref="Unit"/>
        /// </summary>
        private Vector3 ServerPosition3D { get; set; }

        /// <summary>
        /// The cached <see cref="Vector2"/> position of the <see cref="Unit"/>
        /// </summary>
        private Vector2 ServerPosition2D { get; set; }

        /// <summary>
        /// The <see cref="ObjectText"/> entry
        /// </summary>
        private ObjectTextEntry objectTextEntry;

        /// <summary>
        /// The last reset tick
        /// </summary>
        private float LastResetTick { get; set; }

        /// <summary>
        /// The backing field for <see cref="E:Enabled"/>
        /// </summary>
        private bool enabled;

        /// <summary>
        /// Gets the orbwalking unit
        /// </summary>
        public virtual Obj_AI_Base Unit => ObjectCache.Player;

        /// <summary>
        /// The last starting tick of an autoattack
        /// </summary>
        public float LastAttackStartingT { get; private set; }

        #endregion

        #region Disposable methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="managed">Determines whether managed sources should be cleaned</param>
        protected override void Dispose(bool managed)
        {
            if (!managed)
            {
                return;
            }

            this.Enabled = false;

            ObjectText.Entries.Remove(this.objectTextEntry);
            this.objectTextEntry = null;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Fired before the orbwalker - generated attack, useful for cancelling it
        /// </summary>
        public event EventDataHandler<BeforeOrbwalkerAttack> BeforeOrbwalkerAttack;

        /// <summary>
        /// Fired after the windup is done and other actions can be taken. This is a quick callback.
        /// </summary>
        public event EventDataHandler<AfterAttackEventArgs> AfterAttack;

        /// <summary>
        /// Fired when the <see cref="E:Unit" /> isnt't able to execute minions using just basic attacks
        /// </summary>
        public event EventDataHandler<UnkillableMinionsEventArgs> UnkillableMinions;

        #endregion

        #region Event Handlers

        /// <summary>
        /// The OnProcessSpellCast event handler
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event data</param>
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            // Apparently everything controllable should be checked per Player, not per other unit,
            // therefore the IsMe check should be used here and not Comparison()
            if (!sender.IsMe)
            {
                return;
            }

            var name = args.SData.Name.ToLower();

            if (this.IsAutoAttack(name))
            {
                var tick = this.TickCount - this.Latency;

                if (Math.Abs(tick - this.LastAttackStartingT) >= 80f)
                {
                    this.LastAttackStartingT = tick;
                }
            }
            else if (this.IsAutoAttackReset(name))
            {
                this.ResetTimer(true);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            var unit = this.Unit;

            if (unit == null)
            {
                return;
            }

            this.ServerPosition3D = unit.ServerPosition;

            this.ServerPosition2D = this.ServerPosition3D.ToVector2();
        }

        private void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            if (this.Comparison(sender) && AttackResettingBuffs.Contains(args.Buff.DisplayName.ToLower()))
            {
                this.ResetTimer(true);
            }
        }

        private void OnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (this.Comparison(spellbook.Owner) && args.StopAnimation && args.DestroyMissile && !args.ForceStop)
            {
                this.ResetTimer(true);
            }
        }

        #endregion

        #region Protected Instance Methods

        /// <summary>
        /// Determines whether this spell name is an auto-attack
        /// </summary>
        /// <param name="name">The spell name</param>
        /// <returns></returns>
        protected virtual bool IsAutoAttack(string name)
        {
            return AutoAttack.IsAutoAttack(name);
        }

        /// <summary>
        /// Gets the current latency
        /// </summary>
        protected virtual float Latency => Game.Ping / 2f + Menu["misc"]["server_latency"];

        /// <summary>
        /// Determines whether this spell name is an auto-attack reset
        /// </summary>
        /// <param name="name">The spell name</param>
        /// <returns></returns>
        protected virtual bool IsAutoAttackReset(string name)
        {
            return AutoAttack.IsAutoAttack(name);
        }

        /// <summary>
        /// Determines whether the unit will be in the range of Azir's soldiers after W being casted
        /// </summary>
        /// <param name="unit">The requested unit</param>
        /// <returns></returns>
        protected virtual bool InPossibleRange(Obj_AI_Base unit)
        {
            return this.IsMe && Vector2.DistanceSquared(ObjectCache.Player.ServerPosition.ToVector2(), unit.ServerPosition.ToVector2()) <= Soldiers.PossibleRangeSqr;
        }

        /// <summary>
        /// Determines whether a provided <see cref="Obj_AI_Base"/> instance equals the <see cref="Unit"/> of this instance.
        /// </summary>
        /// <param name="base">The provided <see cref="Obj_AI_Base"/> instance</param>
        /// <returns></returns>
        protected virtual bool Comparison(Obj_AI_Base @base) => @base.IsMe;

        /// <summary>
        /// Gets the <see cref="E:Unit"/>'s projectile time to a unit
        /// </summary>
        /// <param name="target">The requested target</param>
        /// <returns></returns>
        protected virtual float ProjectileTime(Obj_AI_Base target)
        {
            var unit = this.Unit;

            var time = unit.AttackCastDelay * 1000f - 100f + Game.Ping / 2f;
            var speed = (unit as Obj_AI_Hero)?.GetProjectileSpeed() ?? (unit.IsMelee ? float.MaxValue : unit.BasicAttack.MissileSpeed);

            if (float.MaxValue - speed > 0.1f)
            {
                time += (unit.Distance(target) - unit.BoundingRadius) * 1000f / speed;
            }

            return time;
        }

        /// <summary>
        /// Determines whether this is currently orbwalking for the <see cref="E:Player"/>
        /// </summary>
        protected bool IsMe => this.Comparison(ObjectCache.Player);

        /// <summary>
        /// Resets the autoattack timer
        /// </summary>
        /// <param name="server">Determines whether the reset has happened in a serversided event</param>
        protected void ResetTimer(bool server)
        {
            var tick = this.TickCount;

            if (server)
            {
                tick -= this.Latency;
            }

            this.LastResetTick = tick;
        }

        #endregion

        #region Enable Handlers 

        // todo change this shit altogether

        private static bool drawing = true;

        /// <summary>
        /// Gets or sets whether the drawing should be enabled
        /// </summary>
        public bool Drawing
        {
            get
            {
                return drawing;
            }

            set
            {
                if (drawing == value)
                {
                    return;
                }

                if (!(drawing = value))
                {
                    Drawings.OnDraw -= OnDraw;
                }
                else
                {
                    Drawings.OnDraw += OnDraw;
                }
            }
        }

        /// <summary>
        /// Gets or sets attacks
        /// </summary>
        public bool Attacks = true;

        /// <summary>
        /// Gets or sets the movement
        /// </summary>
        public bool Movement = true;

        /// <summary>
        /// Determines whether this <see cref="SparkWalker"/> instance has been enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled && (this.Attacks || this.Movement);
            }

            set
            {
                this.enabled = this.Attacks = this.Movement = this.Drawing = value;
            }
        }

        #endregion

        #region Mode Handlers

        /// <summary>
        /// Gets or sets the default custom mode
        /// </summary>
        public Mode DefaultCustomMode;

        /// <summary>
        /// Saves the value of a custom mode
        /// </summary>
        private Mode? customMode;

        /// <summary>
        /// Returns the current <see cref="Enumerations.Mode"/> and as well as allows you to override the settings
        /// </summary>
        public Mode Mode
        {
            get
            {
                return this.customMode ?? EnumCache<Mode>.Values.Find(mode => Menu["keys"][mode.ToString().ToLower()].GetValue<MenuKeyBind>().Active);
            }

            set
            {
                this.customMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the default mode
        /// </summary>
        public bool CustomMode
        {
            get
            {
                return this.customMode.HasValue;
            }

            set
            {
                if (value)
                {
                    this.customMode = this.DefaultCustomMode;
                }
                else
                {
                    this.customMode = null;
                }
            }
        }

        #endregion

        #region Structure

        public SparkWalker()
        {
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            Spellbook.OnStopCast += this.OnStopCast;
            Obj_AI_Base.OnBuffAdd += this.OnBuffAdd;
            Game.OnUpdate += this.OnUpdate;

            ObjectText.Entries.Add(this.objectTextEntry =
                new ObjectTextEntry(x => this.Unit.Compare(x),
                    o => Color.Gold,
                    () => this.CustomMode,
                    o => $"Custom mode: {this.customMode?.ToString() ?? "InvalidOperation"}",
                    "Custom Mode"));

            this.Enabled = true;
        }

        /// <summary>
        /// Initializes the core features of the <see cref="SparkWalker"/>
        /// </summary>
        static SparkWalker()
        {
            var heroes = ObjectCache.GetNative<Obj_AI_Hero>();
            var allies = heroes.FindAll(hero => hero.IsAlly);
            var enemies = heroes.FindAll(hero => hero.IsEnemy);

            Priorities = new List<string>(EnumCache<UnitType>.Count);

            for (var i = 0; i < EnumCache<UnitType>.Count; i++)
            {
                Priorities.Add($"priority_{i}");
            }

            var modesMenu = Menu.Add(new SDKMenu("st_orb_modes", "Modes"));
            {
                foreach (var config in ModeConfiguration)
                {
                    var header = $"st_orb_modes_{config.Key.ToString().ToLower()}";

                    var modeMenu = modesMenu.Add(new Menu(header, config.Key.ToString()));
                    {
                        header += "_targeting";

                        var targetingMenu = modeMenu.Add(new Menu(header, "Targeting"));
                        {
                            var champMenu = targetingMenu.Add(new Menu(header + "_champion", "Champions"));
                            {
                                champMenu.Add(new MenuBool(header + "_champion_ignoreshields", "Ignore shields"));
                                champMenu.Add(new MenuBool(header + "_champion_ignorets", "Ignore TS on targets easy to kill", true));
                                champMenu.Add(new MenuSlider(header + "_champion_attacks", "^ Max killable attacks for this to trigger", 2, 1, 5));

                                var blacklistMenu = champMenu.Add(new Menu(header + "_champion_blacklist", "Blacklist"));
                                {
                                    blacklistMenu.Add(new MenuSeparator(header + "_champion_blacklist_info", "Disable attacking for the following champions:"));

                                    foreach (var hero in enemies)
                                    {
                                        blacklistMenu.Add(new MenuBool(header + "_champion_blacklist" + hero.NetworkId, $"{hero.ChampionName()} ({hero.Name})"));
                                    }
                                }
                            }

                            var objectMenu = targetingMenu.Add(new Menu(header + "_targeting_objects", "Objects"));
                            {
                                objectMenu.Add(new MenuSeparator(header + "_targeting_objects_info", "Attack the following objects:"));
                                objectMenu.AddSeparator();
                                objectMenu.Add(new MenuSeparator(header + "_targeting_objects_wards", "Wards"));

                                foreach (var info in AttackDictionary.Select(pair => pair.Value).Where(info => info.AddToMenu))
                                {
                                    objectMenu.Add(new MenuBool($"{header}_targeting_objects_{info.DisplayName.ToMenuUse()}", info.DisplayName, info.AttackByDefault));
                                }
                            }

                            var structureMenu = targetingMenu.Add(new Menu(header + "_structure", "Structures"));
                            {
                                structureMenu.Add(new MenuBool($"{header}_structures_nexus", "Attack the nexus", true));
                                structureMenu.Add(new MenuBool($"{header}_structures_inhibitor", "Attack inhibitors", true));
                                structureMenu.Add(new MenuBool($"{header}_structures_turret", "Attack turrets", true));
                            }

                            var jungleMenu = targetingMenu.Add(new Menu(header + "_jungle", "Jungle"));
                            {
                                jungleMenu.Add(new MenuBool(header + "_jungle_smallfirst", "Prioritize small minions"));
                            }

                            if (config.Key == Mode.Freeze)
                            {
                                var freezeMenu = targetingMenu.Add(new Menu(header + "_freeze", "Freeze"));
                                {
                                    freezeMenu.Add(new MenuSlider(header + "_freeze_maxhealth", "Health to freeze minions at", 20, 5, 50));
                                }
                            }

                            targetingMenu.AddSeparator();

                            for (var i = 0; i < config.Value.Units.Length; ++i)
                            {
                                targetingMenu.Add(new MenuList<UnitType>($"{header}_priority_{i}", i != 0 ? i != EnumCache<UnitType>.Count - 1 ? $"Priority {i}" : $"Priority {i} (last)" : $"Priority {i} (first)", EnumCache<UnitType>.Values) { SelectedValue = config.Value.Units[i] });
                            }
                        }

                        header = header.Remove("_targeting");

                        modeMenu.Add(new MenuBool(header + "_magnet", "Magnet to champion targets (melee only)", config.Key == Mode.Combo));
                        modeMenu.Add(new MenuBool(header + "_attacks", "Enable attacks", true));
                        modeMenu.Add(new MenuBool(header + "_movement", "Enable movement", true));
                        modeMenu.Add(new MenuKeyBind(header + "_key", $"{config.Key} active!", config.Value.Key, KeyBindType.Press));
                    }
                }

                modesMenu.Add(new MenuKeyBind("st_orb_key_movblock", "Movement block", Key.P, KeyBindType.Press));
            }

            var drawMenu = Menu.Add(new SDKMenu("st_orb_draw", "Drawings"));
            {
                var rangeMenu = drawMenu.Add(new Menu("st_orb_draw_ranges", "Attack ranges"));
                {
                    var adc = rangeMenu.Add(new MenuBool("st_orb_draw_ranges_adc", "Turn on by default for enemy ADC", true));

                    Action<Obj_AI_Hero> addToMenu = hero =>
                    {
                        var id = hero.NetworkId;

                        var heroMenu = rangeMenu.Add(new Menu($"st_orb_draw_ranges_{id}", $"{hero.ChampionName()} ({hero.Name})"));
                        {
                            heroMenu.Add(new MenuSlider($"st_orb_draw_ranges_radius_{id}", "Radius to activate (0 stands for unlimited)", 1500, 0, 5000));
                            heroMenu.Add(new MenuColor($"st_orb_draw_ranges_range_{id}", "Draw range", hero.IsEnemy ? SharpColor.Red : SharpColor.Blue) { Active = adc.Value && hero.IsADC() });
                            heroMenu.Add(new MenuColor($"st_orb_draw_ranges_holdzone_{id}", "Draw HoldZone", SharpColor.White) { Active = false });
                        }
                    };

                    rangeMenu.AddSeparator();
                    rangeMenu.AddSeparator("== ALLIES ==");
                    rangeMenu.AddSeparator();

                    allies.ForEach(addToMenu);

                    rangeMenu.AddSeparator();
                    rangeMenu.AddSeparator("== ENEMIES ==");
                    rangeMenu.AddSeparator();

                    enemies.ForEach(addToMenu);
                }

                var minionMenu = drawMenu.Add(new Menu("minions", "Minions"));
                {
                    minionMenu.Add(new MenuBool("draw_killable_minions", "Draw killable minions", true));
                    minionMenu.Add(new MenuColor("color", "Color", SharpColor.AliceBlue));
                }
            }

            var problemMenu = Menu.Add(new SDKMenu("st_orb_problems", "Problems"));
            {
                problemMenu.Add(new MenuBool("problem_stutter", "I'm stuttering"));
                problemMenu.Add(new MenuBool("problem_missingcs", "The lasthits are badly timed"));
                problemMenu.Add(new MenuBool("problem_holdzone", "The HoldZone isn't big enough"));
            }

            Menu.Add(new MenuBool("attacks_disable", "Disable attacks"));
            Menu.Add(new MenuBool("movement_disable", "Disable movement"));
            Menu.Add(new MenuList<HumanizerMode>("humanizer_mode", "Humanizer mode", EnumCache<HumanizerMode>.Values) { SelectedValue = HumanizerMode.Normal });

            Core.Menu.Add(SDKMenu);

            Game.OnUpdate += delegate
                {
                    if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
                    {
                        return;
                    }

                    if (Utility.Player.IsDashing() || Utility.Player.IsCastingInterruptableSpell(true))
                    {
                        return;
                    }

                    var destination3D = Vector3.Zero;
                    var mode = Mode;
                    var modeMenu = ModeMenu(mode);

                    if (HasAttackPermissions(mode) && CanAttack((ushort)modeMenu["extraMovement"].GetValue<MenuSlider>().Value))
                    {
                        var target = GetTarget(mode);

                        if (target != null)
                        {
                            var args = new BeforeOrbwalkerAttack(target);

                            BeforeUnhumanAttack?.Invoke(args);

                            if (!args.CancelAttack)
                            {
                                ObjectCache.Player.IssueOrder(me ? GameObjectOrder.AttackUnit : GameObjectOrder.AutoAttackPet, args.Target);
                            }
                        }
                    }
                    else if (modeMenu["magnet"] && Unit.IsMelee())
                    {
                        var hero = GetTarget(mode) as Obj_AI_Hero;

                        if (hero != null)
                        {
                            // TODO test
                            var attack = Unit.BasicAttack;

                            var input = new EloBuddy.SDK.Prediction.Manager.PredictionInput
                            {
                                Collision = false,
                                Delay = attack.SpellCastTime,
                                UseBoundingRadius = true,
                                From = ServerPosition3D,
                                Unit = hero,
                                Speed = attack.MissileSpeed,
                                AoE = false,
                                RangeCheckFrom = ServerPosition3D,
                                Radius = 0f,
                                Range = Unit.AttackRange + Unit.BoundingRadius
                            };

                            destination3D = Prediction.GetPrediction(input).UnitPosition;
                        }
                    }

                    if (!HasMovementPermissions(mode) || !CanMove())
                    {
                        return;
                    }

                    if (destination3D.IsZero)
                    {
                        destination3D = Utility.Point();
                    }

                    var destination2D = destination3D.ToVector2();

                    if (Utility.Player.Path.Length > 0)
                    {
                        var holdzone = Utility.Player.BoundingRadius + ExtraHoldZone;

                        if (Vector2.DistanceSquared(Utility.ServerPosition2D, destination2D) < holdzone * holdzone)
                        {
                            Utility.Player.IssueOrder(GameObjectOrder.HoldPosition, Utility.ServerPosition3D);
                            IncreaseTick();
                            return;
                        }
                    }

                    ObjectCache.Player.IssueOrder(me ? GameObjectOrder.MoveTo : GameObjectOrder.MovePet, GetClickPoint(destination2D));

                    IncreaseTick();
                };

            var heroDrawsMenu = Menu[string.Empty];
        }

        #endregion

        #region Methods

        private static void OnDraw(EventArgs args)
        {
            foreach (var hero in ObjectCache.Get<Obj_AI_Hero>())
            {
                var id = hero.NetworkId;

                int radius = heroDrawsMenu[$"st_orb_draw_ranges_radius_{id}"];
                var range = heroDrawsMenu[$"range_color_{id}"].GetValue<MenuColor>();
                var holdZone = heroDrawsMenu[$"holdzone_color_{id}"].GetValue<MenuColor>().Color;

                if (!holdZone.Active && !range.Active)
                {
                    continue;
                }

                if (!hero.IsValidTarget(radius == 0 ? float.MaxValue : radius, false))
                {
                    continue;
                }

                if (range.Active)
                {
                    Drawings.DrawCircle(hero.ServerPosition, hero.AttackRange, range.Color.ToSystemColor());
                }

                if (!holdZone.Active)
                {
                    continue;
                }

                var holdzone = hero.BoundingRadius;

                if (hero.IsMe)
                {
                    holdzone += ExtraHoldZone;
                }

                Drawings.DrawCircle(hero.ServerPosition, holdzone, Menu[$"holdzone_color_{id}"]);
            }

            var drawKillable = Menu["st_orb_draw_minions_killable"].GetValue<MenuColor>();

            if (!drawKillable.Active)
            {
                return;
            }

            ColorBGRA color = Menu["st_orb_draw_minions_killable"];

            foreach (var minion in ObjectCache.GetMinions())
            {
                var position = minion.Position;
                var bounding = minion.BoundingRadius;

                if (!position.IsOnScreen(bounding))
                {
                    continue;
                }

                var alpha = (int)(HealthWrapper.GetPrediction(minion, ProjectileTime(minion)) / AttackDamage(minion) * 255f);

                if (alpha < 255 && alpha > 0)
                {
                    Drawings.DrawCircle(position, bounding, Color.FromArgb(alpha, color.R, color.G, color.B));
                }
            }
        }

        private float AttackTime()
        {
            return this.Unit.AttackDelay * 1000f - 100f + Game.Ping / 2f;
        }

        /// <summary>
        ///     Returns the auto-attack range.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public float GetAttackRange(Obj_AI_Base target)
        {
            if (!this.Unit.IsValid() || !target.IsValidTarget())
            {
                return 0f;
            }

            var result = this.Unit.AttackRange + this.Unit.BoundingRadius + target.BoundingRadius;

            if (this.me && Core.ChampionName == "Caitlyn" && Array.Exists(target.Buffs, buff =>
            {
                if (!buff.Caster?.IsMe ?? true)
                {
                    return false;
                }

                var name = buff.Name;

                return name == "caitlynyordletrapinternal"; // todo get exact net debuff name
            }))
            {
                result += 650f;
            }

            return result;
        }

        /// <summary>
        /// Counts the killhits
        /// </summary>
        /// <param name="target">The requesetd unit</param>
        /// <returns></returns>
        public int KillHits(Obj_AI_Base target)
        {
            return (int)Math.Ceiling(target.Health / this.AttackDamage(target));
        }

        /// <summary>
        /// Returns the damage the <see cref="E:Player"/> can currently deal to a unit
        /// </summary>
        /// <param name="target">The requested unit</param>
        /// <returns></returns>
        public float AttackDamage(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return 0f;
            }

            var damage = 0d;

            if (this.IsMe)
            {
                var soldiers = target.GetSoldiers().Count;

                if (soldiers > 0)
                {
                    damage += ObjectCache.Player.GetSpellDamage(target, SpellSlot.W) * (1d + (soldiers - 1) * 0.25d);
                }
            }

            if (this.GetAttackRange(target) <= this.Unit.Distance(target))
            {
                damage += this.Unit.GetAutoAttackDamage(target);
            }

            return (float)damage;
        }

        private bool CanPlaceSoldier(out int ammo)
        {
            if (!this.IsMe || Core.ChampionName != "Azir")
            {
                ammo = 0;
                return false;
            }

            var instance = ObjectCache.Player.Spellbook.GetSpell(SpellSlot.W);

            ammo = instance.Ammo;

            return instance.State == SpellState.Ready && ammo > 0;
        }

        /// <summary>
        /// Places a soldier on the specified unit
        /// </summary>
        /// <param name="target">The unit</param>
        /// <returns></returns>
        private static bool PlaceSoldier(Obj_AI_Base target)
        {
            var playerPos3D = ObjectCache.Player.ServerPosition;
            var enemyPos3D = target.ServerPosition;
            var enemyPos2D = enemyPos3D.ToVector2();

            var distance = Vector2.Distance(playerPos3D.ToVector2(), enemyPos2D);

            if (distance.Pow() > Soldiers.PossibleRangeSqr)
            {
                return false;
            }

            for (var i = 1f + Random.NextFloat(0f, 1f); i > -0.2f; i -= 1.5f)
            {
                var castPos = playerPos3D.Extend(enemyPos3D, distance * i);

                if (Vector2.Distance(castPos.ToVector2(), enemyPos2D) > Soldiers.WarriorRange + target.BoundingRadius)
                {
                    continue;
                }

                var flags = NavMesh.GetCollisionFlags(castPos);

                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    continue;
                }

                return ObjectCache.Player.Spellbook.CastSpell(SpellSlot.W, castPos);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified unit is in the autoattack range
        /// </summary>
        /// <param name="target">The requested unit</param>
        /// <returns></returns>
        public bool InAttackRange(AttackableUnit target)
        {
            var unit = this.Unit;

            if (unit == null || target == null || !target.IsValid)
            {
                return false;
            }

            var range = (unit.AttackRange + unit.BoundingRadius + target.BoundingRadius).Pow();
            var @base = target as Obj_AI_Base;

            if (@base == null)
            {
                return Vector2.DistanceSquared(this.ServerPosition2D, target.Position.ToVector2()) <= range;
            }

            if (Vector2.DistanceSquared(this.ServerPosition2D, @base.ServerPosition.ToVector2()) <= range)
            {
                return true;
            }

            return unit.NetworkId == ObjectCache.Player.NetworkId && @base.GetSoldiers().Count > 0;
        }

        /// <summary>
        /// Indicates whether the action is allowed for the specified <see cref="Mode"/>
        /// </summary>
        /// <param name="mode">The orbwalking mode</param>
        /// <param name="action">The requested action</param>
        /// <returns></returns>
        private bool HasPermissions(Mode mode, ActionType action)
        {
            if (mode == Mode.None)
            {
                return false;
            }

            bool check;

            switch (action)
            {
                case ActionType.Movement:
                    check = this.Movement && !Menu["keys"]["block_movement"].GetValue<MenuKeyBind>().Active;
                    break;
                case ActionType.Attack:
                    check = this.Attacks;
                    break;
                default:
                    check = false;
                    break;
            }

            if (!check)
            {
                return false;
            }

            var actionName = action.ToString();

            return Menu[actionName + "_enabled"] && ModeMenu(mode)[actionName];
        }

        /// <summary>
        /// Returns a value indicating whether the unit is able to auto-attack
        /// </summary>
        /// <returns></returns>
        private bool CanAttack(Mode mode)
        {
            if (mode == Mode.None)
            {
                return false;
            }

            if (!this.Attacks || !Menu["attack_enabled"] || !ModeMenu(mode)["attack"])
            {
                return false;
            }

            if (!this.Unit.CanAttack)
            {
                return false;
            }

            var buffs = this.Unit.Buffs.ToList();

            if (buffs.Exists(buff => buff.IsValid && buff.Type == BuffType.Blind) || this.AttackPtr != null && !buffs.Exists(this.AttackPtr))
            {
                return false;
            }

            return this.TimeToAttack(mode) < float.Epsilon;
        }

        private float AttackDelay
        {
            get
            {
                if (this.me && Core.ChampionName == "Graves")
                {
                    return this.Unit.AttackDelay * 1074.03f - 716.2381f;
                }

                return this.Unit.AttackDelay * 1000f;
            }
        }

        private static float Speed => 1f - Menu["misc"]["speed"] / 100f;




        public float TimeToAttack(ushort extraMovement)
        {
            var unit = this.Unit;
            var delay = this.AttackDelay;
            var calcLatency = true;

            if (extraMovement != 0)
            {
                delay *= 1f + extraMovement / 1000f;
            }

            var time = this.LastAttackStartingT + delay;

            if (this.LastResetTick > this.LastAttackStartingT)
            {
                var resetT = this.LastResetTick + unit.AttackSpeedMod * 10f * Speed;

                if (resetT > time)
                {
                    time = resetT;
                }
            }

            var buffs = Array.FindAll(unit.Buffs, buff => buff.IsValid).ToList();

            if (buffs.Count > 0)
            {
                var dehancers = buffs.FindAll(buff => buff.Type == BuffType.Blind);
                var endTimes = new List<float>(3);

                if (dehancers.Count > 0)
                {
                    endTimes.Add(dehancers.Max(buff => buff.EndTime));
                }

                if (this.IsMe)
                {
                    switch (Core.ChampionName)
                    {
                        case "Jhin":
                            var reload = buffs.Find(buff => buff.Name.Equals("jhinpassivereload", StringComparison.OrdinalIgnoreCase));

                            if (reload != null)
                            {
                                endTimes.Add(reload.EndTime);
                            }
                            break;
                        case "Graves":

                            break;
                    }
                }

                if (endTimes.Count > 0)
                {
                    var max = endTimes.Max() * 1000f;

                    if (max > time)
                    {
                        time = max;

                        calcLatency = false;
                    }
                }
            }

            if (calcLatency)
            {
                var latency = Game.Ping / 2f - 5f;

                if (latency > 0f)
                {
                    time += latency;
                }
            }

            return Math.Max(time - this.TickCount, 0f);
        }

        public float TimeToAttack(Mode? mode = null)
        {
            var pmode = mode ?? this.Mode;

            return pmode == Mode.None ? float.MaxValue : this.TimeToAttack((ushort)ModeMenu(pmode)["extra_movement"]);
        }

        /// <summary>
        /// Returns a value indicating whether the unit is able to move
        /// </summary>
        /// <returns></returns>
        private bool CanMove()
        {
            if (!this.Unit.CanMove)
            {
                return false;
            }

            var speed = 1f + Menu["misc"]["speed_percent"] / 100f;

            return this.TickCount - this.LastAttackStartingT - this.Unit.AttackCastDelay * 1000f - this.Unit.AttackSpeedMod * 15f * speed > Game.Ping / 2f - 5f;
        }

        /// <summary>
        /// Returns a list of enemy target types this mode can attack
        /// </summary>
        /// <param name="mode">The specified mode</param>
        /// <returns></returns>
        public static HashSet<UnitType> GetTargetSelection(Mode mode)
        {
            var menu = ModeMenu(mode);

            return Priorities.ConvertAll(priority => menu[priority].GetValue<MenuList<UnitType>>().SelectedValue)
                    .ToHashSet();
        }

        /// <summary>
        /// Gets the mode menu
        /// </summary>
        /// <param name="mode">The specified mode</param>
        /// <returns></returns>
        private static AMenuComponent ModeMenu(Mode mode)
        {
            return Menu["st_orb_modes"]["st_orb_mode_" + mode.ToString().ToLower()];
        }

        #endregion

        #region Targeting

        /// <summary>
        /// Gets the target for the specified mode
        /// </summary>
        /// <param name="mode">The specified <see cref="Enumerations.Mode"/></param>
        /// <returns><see cref="AttackableUnit"/></returns>
        public AttackableUnit GetTarget(Mode mode)
        {
            if (mode == Mode.None)
            {
                return null;
            }

            var processed = new HashSet<UnitType> { UnitType.None };
            var menu = ModeMenu(mode);

            return Priorities.Select(priority => menu[priority].GetValue<MenuList<UnitType>>().SelectedValue)
                    .Where(processed.Add)
                    .Select(unitType => this.GetTargetData(unitType, mode, () => processed.Add(UnitType.LaneMinion)))
                    .TakeWhile(data => !data.ShouldWait)
                    .Select(data => data.Target)
                    .FirstOrDefault(target => target != null);
        }

        /// <summary>
        /// Gets the target for the current mode
        /// </summary>
        /// <returns><see cref="AttackableUnit"/></returns>
        public AttackableUnit GetTarget()
        {
            return this.GetTarget(this.Mode);
        }

        private TargetData GetTargetData(UnitType unitType, Mode mode, Predicate checkNormal)
        {
            switch (unitType)
            {
                case UnitType.Champion:
                    return this.GetHero();
                case UnitType.Structure:
                    return this.GetStructure();
                case UnitType.Object:
                    return this.GetAttackableObject();
                case UnitType.LaneMinion:
                    return this.GetKillableMinion(mode == Mode.Freeze);
                case UnitType.LaneClearMinion:
                    return this.GetBalanceMinion(checkNormal(), mode == Mode.Freeze);
                case UnitType.JungleMinion:
                    return this.GetJungleMinion();
                case UnitType.None:
                    return TargetData.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }

        /// <summary>
        /// Gets the best attackable object for the specified mode
        /// </summary>
        /// <returns></returns>
        private TargetData GetAttackableObject()
        {
            var flags = MinionType.Pet;

            if (Menu["targeting"]["objects_wards"])
            {
                flags |= MinionType.Ward;
            }

            ObjectInfo info;

            return new TargetData(
                    from obj in ObjectCache.GetMinions(ObjectTeam.Neutral | ObjectTeam.Enemy, flags, this.InAttackRange)
                    let baseSkinName = obj.CharData.BaseSkinName.ToLower()
                    let ward = baseSkinName.Contains("ward") || baseSkinName.Contains("trinket")
                    where ward || AttackDictionary.TryGetValue(baseSkinName, out info) && info.Attack()
                    orderby ward descending, obj.Health, this.Unit.Distance(obj)
                    select obj);
        }

        /// <summary>
        /// Gets the <see cref="TargetData"/> of the best killable minion
        /// <para>This will cast Azir's W if applicable</para>
        /// </summary>
        /// <param name="freeze">Determines whether to freeze the minions</param>
        /// <returns></returns>
        private TargetData GetKillableMinion(bool freeze)
        {
            const int SoldierAttackTime = 450; // somewhere between 400 and 600 idk

            var minions = ObjectCache.GetMinions();
            var max = freeze ? Menu["targeting"]["freeze_maxhealth"] : int.MaxValue;

            var target = ((from minionData in from minion in minions
                                              where this.InAttackRange(minion)
                                              select new MinionData(minion, max, this.ProjectileTime(minion).Round())
                           where minionData.Prediction <= minionData.Damage
                           orderby minionData.Weight
                           select minionData).FirstOrDefault()
                          ?? (Core.ChampionName != "Azir"
                                  ? null
                                  : minions.Where(
                                      minion =>
                                      Vector2.DistanceSquared(this.ServerPosition2D, minion.ServerPosition.ToVector2())
                                      <= Soldiers.PossibleRangeSqr && !this.InAttackRange(minion))
                                        .Select(minion => new MinionData(minion, max, SoldierAttackTime))
                                        .Where(
                                            minionData =>
                                            minionData.Prediction > 0f
                                            && minionData.Prediction <= StabDamage(minionData.Minion))
                                        .OrderBy(minionData => minionData.Weight)
                                        .Take(2)
                                        .FirstOrDefault(minionData => PlaceSoldier(minionData.Minion))))?.Minion;

            if (target != null)
            {
                return new TargetData(target);
            }

            return new TargetData(minions.Exists(minion =>
            {
                if (!this.InPossibleRange(minion))
                {
                    return false;
                }

                var time = (this.AttackTime() * 2f).Round();
                var damage = this.AttackDamage(minion);

                if (damage < 1f)
                {
                    time += SoldierAttackTime;
                    damage = StabDamage(minion);
                }
                else
                {
                    time += this.ProjectileTime(minion).Round();
                }

                return HealthWrapper.GetPrediction(minion, time, FarmDelay, HealthPredictionType.Simulated) < damage;
            }));
        }

        /// <summary>
        /// Searches for an attackable structure
        /// </summary>
        /// <returns></returns>
        private TargetData GetStructure()
        {
            return new TargetData(Enumerable.Empty<AttackableUnit>()
                .Concat(ObjectCache.Get<Obj_AnimatedBuilding>(ObjectTeam.Enemy, this.InAttackRange))
                .Concat(ObjectCache.Get<Obj_AI_Turret>(ObjectTeam.Enemy, this.InAttackRange))
                .OrderBy(structure => structure.Health));
        }

        /// <summary>
        /// Gets the best hero target
        /// </summary>
        /// <returns></returns>
        private TargetData GetHero()
        {
            int ammo;
            var soldier = this.CanPlaceSoldier(out ammo) && Menu["targeting"]["champions"]["soldier"];
            var pref = (bool)Menu["targeting"]["champions"]["moresoldiers"];

            var targets = Variables.TargetSelector.GetTargets(
                    float.PositiveInfinity,
                    DamageType.Mixed,
                    true,
                    this.ServerPosition3D)
                    .OrderBy(
                        hero =>
                            {
                                var damage = this.AttackDamage(hero);

                                if (soldier)
                                {
                                    var stab = (float)ObjectCache.Player.GetSpellDamage(hero, SpellSlot.W);

                                    if (stab > damage)
                                    {
                                        damage = stab;
                                    }
                                }

                                return Invulnerable.Check(hero, DamageType.Mixed, false, damage);
                            })
                    .ToList();

            var target = targets.Find(this.InAttackRange);

            if (target != null)
            {
                if (soldier && pref && ammo > 1)
                {
                    PlaceSoldier(target);
                }

                return new TargetData(target);
            }

            if (!soldier || ((target = targets.Find(this.InPossibleRange)) == null))
            {
                return TargetData.Empty;
            }

            PlaceSoldier(target);
            return new TargetData(target);
        }

        /// <summary>
        /// Gets a lane balance minion
        /// </summary>
        /// <param name="freeze">Determines whether to freeze the minions</param>
        /// <param name="checkNormal">Determines whether to freeze the minions</param>
        /// <returns></returns>
        private TargetData GetBalanceMinion(bool checkNormal, bool freeze)
        {
            // ReSharper disable once InvertIf
            if (checkNormal)
            {
                var normal = this.GetKillableMinion(freeze);

                if (normal.ShouldWait || normal.Target != null)
                {
                    return normal;
                }
            }

            return new TargetData(from minion in ObjectCache.GetMinions(ObjectTeam.Enemy, MinionType.Minion, this.InAttackRange)
                                  let pred = HealthWrapper.GetPrediction(minion, (this.AttackTime() * 2f + this.ProjectileTime(minion)).Round(), FarmDelay)
                                  let damage = this.AttackDamage(minion)
                                  where pred >= 2f * damage || Math.Abs(pred - minion.Health) < 1f
                                  orderby minion.MaxHealth descending, this.Unit.DistanceSquared(minion)
                                  select minion);
        }

        /// <summary>
        /// Gets the jungle minion
        /// </summary>
        /// <returns></returns>
        private TargetData GetJungleMinion()
        {
            bool smallfirst = Menu["targeting"]["jungle"]["targetsmallfirst"];

            return new TargetData(
                from minion in ObjectCache.GetMinions(ObjectTeam.Neutral, MinionType.Jungle, this.InAttackRange)
                let type = minion.DetermineType()
                orderby type == AIMinionType.JungleSmall == smallfirst descending, type descending, minion.Health, this.Unit.Distance(minion)
                select minion);
        }

        #endregion

        #region Azir Wrappers

        /// <summary>
        /// The full attack range of the soldier
        /// </summary>
        public const int WarriorRange = 350;

        /// <summary>
        /// The full attack range of the soldier, squared
        /// </summary>
        public const int WarriorRangeSqr = WarriorRange * WarriorRange;

        /// <summary>
        /// The possible range of W, squared
        /// </summary>
        public const int PossibleRangeSqr = 640000;

        private static ObjectTracker<Obj_AI_Minion> SoldierTracker => SoldierManager.GetTracker(ObjectCache.Player);

        public static List<Obj_AI_Minion> Soldiers => SoldierTracker.Items;

        public static List<Obj_AI_Minion> SoldiersCanAttack
        {
            get
            {
                var playerPos = ObjectCache.Player.ServerPosition.To2D();

                return Soldiers.FindAll(soldier => !soldier.IsMoving && soldier.Distance(playerPos) <= 800F);
            }
        }

        public static List<Obj_AI_Minion> GetSoldiers(Obj_AI_Base target, )
        {
            // Assumptions for the player to attack a target using soldiers:
            // Player is Azir
            // The target can be attacked
            // Target is a hero, a lane or a jungle minion
            // Target is no more than 1000 units from player
            // Target is no more than 350 units from the soldier
            // Soldier is no more than 800 units from player
            // Soldier isn't moving

            if (!target.IsValidTarget())
            {
                return MiscallenousCache.GetEmptyList<Obj_AI_Minion>();
            }

            var position = target.ServerPosition.To2D();

            if (Vector2.DistanceSquared(ObjectCache.Player.ServerPosition.To2D(), position) > 1000000F)
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

            return SoldiersCanAttack.FindAll(soldier => soldier.Distance(position, true) <= WarriorRangeSqr);
        }

        #endregion
    }
}*/