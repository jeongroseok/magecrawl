using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    internal class Slow : EffectResult
    {
        private double m_modifier;

        public Slow()
        {
        }

        public Slow(int strength)
        {
            m_modifier = 1.4;
        }

        internal override void Apply(Character appliedTo)
        {
            appliedTo.CTIncreaseModifier /= m_modifier;
        }

        internal override void Remove(Character removedFrom)
        {
            removedFrom.CTIncreaseModifier *= m_modifier;
        }

        internal override string Name
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
