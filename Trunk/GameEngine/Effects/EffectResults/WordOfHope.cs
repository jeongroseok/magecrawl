using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    class WordOfHope : EffectResult
    {
        private int m_level;

        public WordOfHope()
        {
            m_level = 0;
        }

        public WordOfHope(int strength, Character caster)
        {
            m_level = strength;
        }

        internal override string Name
        {
            get
            {
                return "Hope";
            }
        }

        // Needs to match class name
        internal override string Type
        {
            get 
            {
                return "WordOfHope"; 
            }
        }

        private int BonusStamina
        {
            get
            {
                return 60 + (20 * (int)Math.Max(0, m_level - 2));
            }
        }

        internal override void Apply(Character appliedTo)
        {
            appliedTo.Heal(BonusStamina, false);
        }

        internal override void Remove(Character removedFrom)
        {
            removedFrom.DamageStamina(BonusStamina);
        }

        public override string GetAttribute(string key)
        {
            if (key == "BonusWeaponDamage")
                return (2 + (int)Math.Max(0, m_level - 2)).ToString();
            else if (key == "BonusStamina")
                return BonusStamina.ToString();
            throw new KeyNotFoundException();
        }

        public override bool ContainsKey(string key)
        {
            return key == "BonusWeaponDamage" || key == "BonusStamina";
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
                return (new DiceRoll(1, 7, 7)).Roll() * CoreTimingEngine.CTNeededForNewTurn;    //8-15 turns
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
