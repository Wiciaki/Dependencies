namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Drawing;

    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Executors;

    /// <summary>
    /// This class offers you to draw text under the objects in an easy way
    /// </summary>
    [Trigger]
    public class ObjectText
    {
        /// <summary>
        /// The <see cref="ObservableCollection{T}"/> of champion texts to be drawn
        /// </summary>
        public static readonly ObservableCollection<ObjectTextEntry> Entries;

        /// <summary>
        /// Determined the height difference between consecutive drawn texts
        /// </summary>
        private const int StepSize = 25;

        /// <summary>
        /// Holds the text Menu item
        /// </summary>
        private static readonly Menu Menu;

        /// <summary>
        /// Adds an item to the <see cref="E:TextHandlerMenu"/>
        /// </summary>
        /// <param name="item">The <see cref="ObjectText"/>'s component to be added</param>
        private static void AddItem(ObjectTextEntry item)
        {
            Menu.Add($"text_{item.Id}", new CheckBox($"Enable \"{item.MenuText}\"", item.OnByDefault));
        }

        /// <summary>
        /// Removes an item from the <see cref="E:TextHandlerMenu"/>
        /// </summary>
        /// <param name="item">The <see cref="ObjectText"/>'s component to be removed</param>
        private static void RemoveItem(ObjectTextEntry item)
        {
            Menu.Remove($"text_{item.Id}");
        }

        /// <summary>
        /// Initializes static members of the <see cref="ObjectText"/> class
        /// </summary>
        static ObjectText()
        {
            Menu = Variables.SDKMenu.AddSubMenu("Text below units", "st_core_drawings_text");
            Menu.Add("text_enable", new CheckBox("Enable"));

            Entries = new ObservableCollection<ObjectTextEntry>();

            Entries.CollectionChanged += (sender, args) =>
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            AddItem((ObjectTextEntry)args.NewItems[0]);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            RemoveItem((ObjectTextEntry)args.OldItems[0]);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            AddItem((ObjectTextEntry)args.NewItems[0]);
                            RemoveItem((ObjectTextEntry)args.OldItems[0]);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                        case NotifyCollectionChangedAction.Move:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(args.Action), args.Action, null);
                    }
                };

            Drawing.OnDraw += delegate
                {
                    if (!Menu["text_enable"].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }

                    var entries =
                        Entries.Where(item => Menu[$"text_{item.Id}"].Cast<CheckBox>().CurrentValue && item.Condition())
                            .OrderBy(item => item.Id)
                            .ToList();

                    if (entries.Count == 0)
                    {
                        return;
                    }

                    foreach (var o in ObjectCache.GetNative<GameObject>())
                    {
                        var current = entries.FindAll(item => item.Draw(o));

                        if (current.Count == 0)
                        {
                            continue;
                        }

                        var pos = Drawing.WorldToScreen(o.Position);
                        var steps = 0;

                        foreach (var item in current)
                        {
                            var text = item.DrawnText(o);

                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                Drawing.DrawText(
                                    pos.X - text.Length * 5, 
                                    pos.Y - steps++ * StepSize, 
                                    item.Color(o), 
                                    text);
                            }
                        }
                    }
                };
        }
    }

    /// <summary>
    /// The <see cref="ObjectTextEntry"/> class
    /// </summary>
    public class ObjectTextEntry
    {
        /// <summary>
        /// Responsible for delivering the Ids
        /// </summary>
        private static ushort id;

        /// <summary>
        /// The <see cref="Func{TResult}"/> whether to draw on the specified <see cref="GameObject"/>
        /// </summary>
        internal readonly Predicate<GameObject> Draw;

        /// <summary>
        /// The <see cref="System.Drawing.Color"/> <see cref="Func{TResult}"/>
        /// </summary>
        internal readonly Func<GameObject, Color> Color;

        /// <summary>
        /// The condition <see cref="Func{TResult}"/>
        /// </summary>
        internal readonly Predicate Condition;

        /// <summary>
        /// The drawing <see cref="E:id"/>
        /// </summary>
        internal readonly ushort Id;

        /// <summary>
        /// The text to be drawn
        /// </summary>
        internal readonly Func<GameObject, string> DrawnText;

        /// <summary>
        /// The text to appear in the Menu
        /// </summary>
        internal readonly string MenuText;

        /// <summary>
        /// Indicates whether this item should be enabled by default
        /// </summary>
        internal readonly bool OnByDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectText"></see> class
        /// </summary>
        /// <param name="drawOnObject">The <see cref="Func{TResult}"/> whether to draw on the specified <see cref="GameObject"/></param>
        /// <param name="color">The <see cref="System.Drawing.Color"/> <see cref="Func{TResult}"/></param>
        /// <param name="condition">The condition <see cref="Func{TResult}"/></param>
        /// <param name="drawnText">The text to be drawn</param>
        /// <param name="menuText">The text to appear in the Menu</param>
        /// <param name="onByDefault">Indicates whether this item should be enabled by default</param>
        public ObjectTextEntry(
            Predicate<GameObject> drawOnObject, 
            Func<GameObject, Color> color, 
            Predicate condition, 
            Func<GameObject, string> drawnText, 
            string menuText, 
            bool onByDefault = true)
        {
            this.Draw = drawOnObject ?? (o => false);

            this.Color = color ?? (o => System.Drawing.Color.White);

            this.Condition = condition ?? (() => false);

            this.DrawnText = drawnText ?? (o => null);

            this.MenuText = menuText ?? "Unnamed item";

            this.OnByDefault = onByDefault;

            this.Id = id++;
        }
    }
}