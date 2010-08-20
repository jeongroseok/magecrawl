using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;
using System.Collections.Generic;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    internal class Slow : EffectResult
    {
        private double m_modifier;

        public Slow()
        {
        }

        public Slow(int strength, Character caster)
        {
            m_modifier = 1.10 + (.1 * strength);
        }

        public override string GetAttribute(string key)
        {
            if (key == "CTIncreaseModifierBonus")
                return m_modifier.ToString();
            throw new KeyNotFoundException();
        }

        public override bool ContainsKey(string key)
        {
            return key == "CTIncreaseModifierBonus";
        }

        internal override string Name
        {
            get
            {
                return "Slow";
            }
        }

        // Needs to match class name
        internal override string Type
        {
            get
            {
                return "Slow";
            }
        }

        internal override bool IsPositiveEffect
        {
            get
            {
                return false;
            }
        }

        internal override int DefaultEffectLength
        {
            get
            {
                return (new DiceRoll(1, 8, 4)).Roll() * CoreTimingEngine.CTNeededForNewTurn;    //5-12 turns
            }
        }

        #region SaveLoad

        internal override void ReadXml(System.Xml.XmlReader reader)
        {
            m_modifier = reader.ReadElementContentAsDouble();
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Modifier", m_modifier.ToString());
        }

        #endregion
    }
}
