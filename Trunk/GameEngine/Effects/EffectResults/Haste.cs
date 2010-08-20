using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;
using System.Collections.Generic;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    internal class Haste : EffectResult
    {
        private double m_modifier;

        public Haste()
        {
        }

        public Haste(int strength, Character caster)
        {
            m_modifier = 1.2 + (.1 * strength);
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
                return "Haste";
            }
        }

        // Needs to match class name
        internal override string Type
        {
            get
            {
                return "Haste";
            }
        }

        internal override bool IsPositiveEffect
        {
            get
            {
                return true;
            }
        }

        internal override int DefaultMPSustainingCost
        {
            get
            {
                return 40;
            }
        }

        internal override int DefaultEffectLength
        {
            get
            {
                return (new DiceRoll(1, 5, 10)).Roll() * CoreTimingEngine.CTNeededForNewTurn;    //10-15 turns
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
