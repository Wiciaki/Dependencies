/*
namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Properties;

    /// <summary>
    /// Manages the translation data
    /// </summary>
    [Trigger]
    public static class Translations
    {
        /// <summary>
        /// Contains the cached <see cref="CultureInfo"/> instances for every <see cref="Language"/>
        /// </summary>
        private static readonly Dictionary<Language, CultureInfo> LanguageInfos;

        /// <summary>
        /// Initializes static members of the <see cref="Translations"/> class
        /// </summary>
        static Translations()
        {
            LanguageInfos = EnumCache<Language>.Values.ToDictionary(
                language => language, 
                language => new CultureInfo(EnumCache<Language>.Description(language)));

            var menu = new Menu("st_core_translations", "Translations");
            {
                var language = menu.Add(new MenuList<Language>("selected_language", "Selected language", EnumCache<Language>.Values));

                if (Core.FirstRun)
                {
                    language.SelectedValue = CurrentLanguage;
                }

                language.ValueChanged += delegate
                {
                    var value = LanguageInfos[language.SelectedValue];

                    if (CultureInfo.CurrentUICulture.Equals(value))
                    {
                        return;
                    }

                    CultureInfo.CurrentUICulture = value;

                    UpdateAll();
                };
            }
        }

        /// <summary>
        /// Gets the currently used language if it's supported, otherwise <c><see cref="Language.English"/></c>
        /// </summary>
        public static Language CurrentLanguage => (from pair in LanguageInfos where pair.Value.Equals(CultureInfo.CurrentUICulture) select pair.Key).SingleOrDefault();

        /// <summary>
        /// Gets a raw translation from the resource files using the key and an optional language
        /// </summary>
        /// <param name="name"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetTranslation(string name, Language? language)
        {
            return language.HasValue
                       ? Resources.ResourceManager.GetString(name, LanguageInfos[language.Value])
                       : Resources.ResourceManager.GetString(name);
        }

        /// <summary>
        /// Contains the pointers to current values of the keys
        /// </summary>
        private static readonly Dictionary<string, Func<string>> Replacements = new Dictionary<string, Func<string>>();

        /// <summary>
        /// Registers a new replacement
        /// </summary>
        /// <param name="key">The key to be seeked for</param>
        /// <param name="replacement">The string to replace the key when matched</param>
        internal static void RegisterReplacement(string key, Func<object> replacement)
        {
            Replacements.Add(key, () =>
                {
                    var result = replacement();

                    return result == null ? "null" : (result as string ?? result.ToString());
                });
        }

        /// <summary>
        /// The default flags for the <see cref="Regex"/> instances
        /// </summary>
        private const RegexOptions Flags = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        /// <summary>
        /// The regular expression
        /// </summary>
        private static readonly Regex BracesAroundTextRegex = new Regex(@"{\w+}", Flags);

        /// <summary>
        /// The regular expression
        /// </summary>
        private static readonly Regex BraceFinderRegex = new Regex(@"[{}]", Flags);

        /// <summary>
        /// The regular expression
        /// </summary>
        private static readonly Regex NumberMatcherRegex = new Regex(@"\A-?\d+\z", Flags);

        /// <summary>
        /// Updates the display name of a specified menu component to match the new data
        /// </summary>
        /// <param name="component">The <see cref="AMenuComponent"/> instance to be updated</param>
        /// <param name="language">The specified language</param>
        /// <returns></returns>
        public static void Update(AMenuComponent component, Language? language = null)
        {
            var name = component.Name;

            if (NumberMatcherRegex.IsMatch(name))
            {
                return;
            }

            var translation = GetTranslation(name, language);

            if (translation == null)
            {
                Console.WriteLine($"A translation was not included! Name: {name}");
                return;
            }

            try
            {
                component.DisplayName = (from Match match in BracesAroundTextRegex.Matches(translation)
                                         select match.Groups).SelectMany(
                                             groups =>
                                             {
                                                 var list = new List<string>(groups.Count);

                                                 for (var i = 0; i < groups.Count; i++)
                                                 {
                                                     list.Add(groups[i].Value);
                                                 }

                                                 return list;
                                             })
                    .Aggregate(
                        translation, 
                        (current, match) =>
                        current.Replace(match, Replacements[BraceFinderRegex.Replace(match, string.Empty)]()));
            }
            catch (KeyNotFoundException ex)
            {
                ex.Catch($"Couldn't translate {name}");
            }
        }

        /// <summary>
        /// Updates all of the menu components
        /// </summary>
        public static void UpdateAll(Language? language = null)
        {
            foreach (var component in MenuH.Components)
            {
                Update(component, language);
            }
        }
    }
}*/