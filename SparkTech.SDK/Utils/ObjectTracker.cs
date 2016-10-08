namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;

    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Cache;

    public class ObjectTracker<T> : Executable where T : Obj_AI_Base
    {
        public readonly List<T> Items;

        public event Action<T> Created, Deleted;

        private bool process;

        private readonly string itemName, spellName;

        private readonly StringComparison comparison;

        private readonly int trackedId;
        
        public ObjectTracker(string itemName, string spellName, int? sourceNetworkId = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            this.Items = new List<T>();
            this.itemName = itemName;
            this.spellName = spellName;
            this.comparison = comparison;
            this.trackedId = sourceNetworkId ?? ObjectCache.Player.NetworkId;

            GameObject.OnCreate += this.OnCreate;
            Obj_AI_Base.OnPlayAnimation += this.OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.NetworkId == this.trackedId && this.spellName.Equals(args.SData.Name, this.comparison))
            {
                this.process = true;
            }
        }

        private void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!this.itemName.Equals(sender.Name, this.comparison) || args.Animation != "Death")
            {
                return;
            }

            var index = this.Items.FindIndex(item => item.NetworkId == sender.NetworkId);

            if (index < 0)
                return;

            var removedItem = this.Items[index];
            this.Items.RemoveAt(index);
            this.Deleted?.Invoke(removedItem);
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (!this.process || !sender.Name.Equals(this.itemName, this.comparison))
            {
                return;
            }

            this.process = false;
            var item = (T)sender;
            this.Items.Add(item);
            this.Created?.Invoke(item);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="managed">Determines whether managed sources should be cleaned</param>
        protected override void Dispose(bool managed)
        {
            GameObject.OnCreate -= this.OnCreate;
            Obj_AI_Base.OnPlayAnimation -= this.OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast -= this.OnProcessSpellCast;

            if (managed)
            {
                this.Items.Clear();
                this.Items.TrimExcess();
            }

            this.Created = null;
            this.Deleted = null;
        }
    }
}