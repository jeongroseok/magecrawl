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
        private int m_strength;
        private string m_targetType;
        private string m_school;

        internal Spell(string name, string school, string effectType, int cost, int strength, string targetType)
        {
            m_name = name;
            m_effectType = effectType;
            m_cost = cost;
            m_strength = strength;
            m_targetType = targetType;
            m_school = school;
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
                return Name + '\t' + m_school + '\t' + "Mp: " + m_cost.ToString();
            }
        }

        public string School
        {
            get
            {
                return m_school;
            }
        }

        public string TargetType
        {
            get 
            {
                return m_targetType;
            }
        }

        internal string EffectType
        {
            get
            {
                return m_effectType;
            }
        }

        internal int Cost
        {
            get
            {
                return m_cost;
            }
        }

        internal int Strength
        {
            get
            {
                return m_strength;
            }
        }
    }
}
