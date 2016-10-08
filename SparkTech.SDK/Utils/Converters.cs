namespace SparkTech.SDK.Utils
{
    using System;

    public static class Converters
    {
        /// <summary>
        /// Rounds the float to the nearest <see cref="int"/> value
        /// </summary>
        /// <param name="value">The floating point-number</param>
        /// <returns></returns>
        public static int Round(this float value)
        {
            return Round((double)value);
        }

        /// <summary>
        /// Rounds the double to the nearest <see cref="int"/> value
        /// </summary>
        /// <param name="value">The floating point-number</param>
        /// <returns></returns>
        public static int Round(this double value)
        {
            return (int)Math.Round(value, MidpointRounding.ToEven);
        }

        /// <summary>
        /// Returns a floating point number increased to the ^2 power
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public static float Pow(this float value)
        {
            return value * value;
        }

        public static double Root(this double value, double level = 2d)
        {
            return Math.Pow(value, 1d / level);
        }

        public static int ToTicks(this float seconds)
        {
            return (seconds * 1000f).Round();
        }

        public static float ToSeconds(this int ticks)
        {
            return ticks / 1000f;
        }
    }
}