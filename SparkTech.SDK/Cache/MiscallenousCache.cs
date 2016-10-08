namespace SparkTech.SDK.Cache
{
    using System.Collections.Generic;

    internal class MiscallenousCache
    {
        public static List<T> GetEmptyList<T>()
        {
            // ReSharper disable once InvertIf
            if (ListCache<T>.Instance.Count != 0)
            {
                ListCache<T>.Instance.Clear();
                ListCache<T>.Instance.TrimExcess();
            }

            return ListCache<T>.Instance;
        }

        private static class ListCache<T>
        {
            internal static readonly List<T> Instance;

            static ListCache()
            {
                Instance = new List<T>(0);
                Instance.TrimExcess();
            }
        }
    }
}