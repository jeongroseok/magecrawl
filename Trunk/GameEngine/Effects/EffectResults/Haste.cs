using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    internal class Haste : EffectResult
    {
        private double m_modifier;

        public Haste()
        {
        }

        public Haste(int strength)
        {
            m_modifier = 1.25;
        }

        internal override void Apply(Character appliedTo)
        {
            appliedTo.CTIncreaseModifier *= m_modifier;
        }

        internal override void Remove(Character removedFrom)
        {
            removedFrom.CTIncreaseModifier /= m_modifier;
        }

        internal override string Name
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
