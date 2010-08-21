using System;
using System.Collections.Generic;
using Magecrawl.EngineInterfaces;
using Magecrawl.Utilities;

namespace Magecrawl.StatusEffects.EffectResults
{
    class ArmorOfLight : EffectResult
    {
        private int m_level;

        public ArmorOfLight()
        {
            m_level = 0;
        }

        public ArmorOfLight(int strength, ICharacterCore caster)
        {
            m_level = strength;
        }

        internal override string Name
        {
            get
            {
                return "Armor Of Light";
            }
        }

        // Needs to match class name
        internal override string Type
        {
            get
            {
                return "ArmorOfLight";
            }
        }

        public override string GetAttribute(string key)
        {
            if (key == "DamageReduction")
                return (3 + (int)Math.Max(0, m_level - 1)).ToString();
            throw new KeyNotFoundException();
        }

        public override bool ContainsKey(string key)
        {
            return key == "DamageReduction";
        }

        internal override bool IsPositiveEffect
        {
            get
            {
                return true;
            }
        }

        internal override int DefaultEffectLength
        {
            get
            {
                return (new DiceRoll(1, 11, 14)).Roll() * TimeConstants.CTNeededForNewTurn;    //15-25 turns
            }
        }

        internal override int DefaultMPSustainingCost
        {
            get
            {
                return 35;
            }
        }

        internal override void ReadXml(System.Xml.XmlReader reader)
        {
            m_level = reader.ReadElementContentAsInt();
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Level", m_level.ToString());
        }
    }
}
