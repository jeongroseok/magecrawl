using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;

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

        internal Spell(string name, string school, string effectType, int cost, TargetingInfo.TargettingType targettingType, int range)
        {
            m_name = name;
            m_effectType = effectType;
            m_cost = cost;
            m_school = school;
            m_targettingType = targettingType;
            m_range = range;
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
                return Name + '\t' + m_school + '\t' + "Mp: " + Cost.ToString();
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

        private double GetArmorCostPenaltiy(IArmor armor)
        {
            const double StandardWeightPenality = .125;
            const double HeavyWeightPenality = .25;

            if (armor == null)
                return 0;

            if (armor.Weight == ArmorWeight.Standard)
                return StandardWeightPenality;
            if (armor.Weight == ArmorWeight.Heavy)
                return HeavyWeightPenality;
            return 0;
        }

        private int CalculateSpellCost(Player caster)
        {
            double penalityForHeavyArmor = 0;
            penalityForHeavyArmor += GetArmorCostPenaltiy(caster.Boots);
            penalityForHeavyArmor += GetArmorCostPenaltiy(caster.ChestArmor);
            penalityForHeavyArmor += GetArmorCostPenaltiy(caster.Gloves);
            penalityForHeavyArmor += GetArmorCostPenaltiy(caster.Headpiece);
            return (int)Math.Round(m_cost * (1.0 + penalityForHeavyArmor));
        }

        internal int Cost
        {
            get
            {
                return CalculateSpellCost(CoreGameEngine.Instance.Player);
            }
        }

        public TargetingInfo Targeting
        {
            get 
            {
                return new TargetingInfo(m_targettingType, m_range);
            }
        }
    }
}
