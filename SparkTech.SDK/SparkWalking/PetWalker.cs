/*
namespace SparkTech.SparkWalking
{
    using System.Collections.Generic;

    /// <summary>
    /// The orbwalker where the <see cref="Unit"/> is <see cref="ObjectManager.Player"/>'s pet
    /// </summary>
    internal sealed class PetWalker : SparkWalker
    {
        private static readonly List<string> PetAttacks = new List<string> { "annietibbersbasicattack", "annietibbersbasicattack2" };

        /// <summary>
        /// Gets the orbwalking unit
        /// </summary>
        public override Obj_AI_Base Unit => base.Unit.Pet as Obj_AI_Minion;

        /// <summary>
        /// Gets the attack order
        /// </summary>
        protected override GameObjectOrder AttackOrder => GameObjectOrder.MovePet;

        /// <summary>
        /// Gets the move order
        /// </summary>
        protected override GameObjectOrder MoveOrder => GameObjectOrder.MovePet;

        /// <summary>
        /// Gets the stop order
        /// </summary>
        protected override GameObjectOrder StopOrder => GameObjectOrder.MovePet;

        /// <summary>
        /// Determines whether this spell name is an auto-attack
        /// </summary>
        /// <param name="name">The spell name</param>
        /// <returns></returns>
        protected override bool IsAutoAttack(string name)
        {
            return PetAttacks.Contains(name);
        }

        /// <summary>
        /// Determines whether this spell name is an auto-attack reset
        /// </summary>
        /// <param name="name">The spell name</param>
        /// <returns></returns>
        protected override bool IsAutoAttackReset(string name)
        {
            // Pets don't cast spells
            return false;
        }

        /// <summary>
        /// Determines whether a provided <see cref="Obj_AI_Base"/> instance equals the <see cref="SparkWalker.Unit"/> of this instance.
        /// </summary>
        /// <param name="base">The provided <see cref="Obj_AI_Base"/> instance</param>
        /// <returns></returns>
        protected override bool Comparison(Obj_AI_Base @base)
        {
            var unit = this.Unit;

            return unit.IsValid() && unit.NetworkId == @base.NetworkId;
        }
    }
}*/