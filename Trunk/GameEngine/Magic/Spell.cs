using System;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Effects;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Magic
{
    internal sealed class Spell : ISpell
    {
        private string m_name;
        private string m_effectType;
        private int m_cost;
        private string m_school;
        private TargetingInfo.TargettingType m_targettingType;
        private int m_range;

        internal Spell(string name, string school, string effectType, int cost, TargetingInfo.TargettingType targettingType, int range, DiceRoll baseDamage, DiceRoll damagePerLevel)
        {
            m_name = name;
            m_effectType = effectType;
            m_cost = cost;
            m_school = school;
            m_targettingType = targettingType;
            m_range = range;
            BaseDamage = baseDamage;
            DamagePerLevel = damagePerLevel;
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public string DisplayName
        {
            get
            {
                if (SustainingCost > 0)
                    return string.Format("{0}\t{1}\tMP:{2}(+{3})", Name, m_school, Cost, SustainingCost);
                else
                    return string.Format("{0}\t{1}\tMP:{2}", Name, m_school, Cost);
            }
        }

        public string School
        {
            get
            {
                return m_school;
            }
        }

        internal string EffectType
        {
            get
            {
                return m_effectType;
            }
        }

        public DiceRoll BaseDamage { get; private set; }
        public DiceRoll DamagePerLevel { get; private set; }

        private double GetArmorCostPenaltiy(IArmor armor)
        {
            const double StandardWeightPenality = .2;
            const double HeavyWeightPenality = .5;

            if (armor == null)
                return 0;
            else if (armor.Weight == ArmorWeight.Standard)
                return StandardWeightPenality;
            else if (armor.Weight == ArmorWeight.Heavy)
                return HeavyWeightPenality;
            else
                return 0;
        }

        private double CalculateSpellPenalityForArmor(Player caster)
        {
            double penalityForHeavyArmor = 0;
            penalityForHeavyArmor += GetArmorCostPenaltiy(caster.Boots);
            penalityForHeavyArmor += GetArmorCostPenaltiy(caster.ChestArmor);
            penalityForHeavyArmor += GetArmorCostPenaltiy(caster.Gloves);
            penalityForHeavyArmor += GetArmorCostPenaltiy(caster.Headpiece);
            return 1.0 + penalityForHeavyArmor;
        }

        internal int Cost
        {
            get
            {
                double armorPenality = CalculateSpellPenalityForArmor(CoreGameEngine.Instance.Player);
                int costReduction = CoreGameEngine.Instance.Player.GetTotalAttributeValue(m_school + "ReducedCost");
                int cost = (int)Math.Round((m_cost - costReduction) * armorPenality);
                if (cost <= 0)
                    throw new InvalidOperationException("Can't have zero or negative cost for a spell - " + m_name);
                return cost;
            }
        }

        // Similar logic is found in Player::MaxMP
        internal int SustainingCost
        {
            get
            {
                double sustainingCostPercentage = 0;
                LongTermEffect possibleEffect = CoreGameEngine.Instance.GetLongTermEffectSpellWouldProduce(EffectType);
                if (possibleEffect != null)
                    sustainingCostPercentage = possibleEffect.MPCost;
                return (int)Math.Round((sustainingCostPercentage / 100.0) * CoreGameEngine.Instance.Player.MaxPossibleMP);
            }
        }

        public TargetingInfo Targeting
        {
            get 
            {
                int rangeBonus = CoreGameEngine.Instance.Player.GetTotalAttributeValue(m_name + "IncreasedRange");
                return new TargetingInfo(m_targettingType, m_range + rangeBonus);
            }
        }
    }
}
