/*
namespace SparkTech.SparkWalking
{
    using System;


    public class MinionData
    {
        public readonly Obj_AI_Minion Minion;

        public readonly MinionTypes Type;

        public readonly float Damage;

        public readonly float Prediction;

        public readonly float Weight;

        public MinionData(Obj_AI_Minion minion, int max, int time)
        {
            this.Minion = minion;

            this.Damage = Math.Min(SparkWalker.AttackDamage(minion), max);

            this.Type = minion.GetMinionType();

            this.Prediction = HealthWrapper.GetPrediction(this.Minion, time, SparkWalker.FarmDelay);

            this.Weight = Math.Abs(this.Prediction / (HealthWrapper.GetAggroCount(minion) / 2.4f + 1f));

            if (this.Prediction < 0f)
            {
                this.Weight *= 2.8f;
            }

            if (HealthWrapper.HasTurretAggro(minion))
            {
                this.Weight /= 2.4f;
            }

            if (this.Type.HasFlag(MinionTypes.Siege))
            {
                this.Weight /= 1.8f;
            }
            else if (this.Type.HasFlag(MinionTypes.Melee))
            {
                this.Weight /= 1.2f;
            }
        }
    }
}
*/