/*
namespace SparkTech.SDK.Targeting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    public class Weight
    {
        internal static IEnumerable<Slider> GenerateWeights()
        {
            TargetAcquire.Weights.AddRange(
                Variables.Assembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(Weight)))
                    .OrderBy(type => type.Name)
                    .Select(type => (Weight)Activator.CreateInstance(type)));

            return TargetAcquire.Weights.ConvertAll(weight => weight.Item);
        }

        private int modifier;

        private readonly Slider Item;

        public Weight(Slider item)
        {
            this.modifier = item.CurrentValue;

            this.Item = item;

            this.Item.OnValueChange += (sender, args) =>
                {
                    this.modifier = args.NewValue;
                };
        }
    }
}
*/