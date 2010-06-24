using System.ComponentModel.Composition;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    [Export(typeof(EffectResult))]
    [ExportMetadata("Name", "Haste")]
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

        internal override int DefaultMPSustainingCost
        {
            get
            {
                return 5;
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
