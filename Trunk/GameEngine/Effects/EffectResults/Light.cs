using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    internal class Light : EffectResult
    {
        private int m_visionBoost;

        public Light()
        {
        }

        public Light(int strength)
        {
            m_visionBoost = strength / 2;
            if (m_visionBoost < 2)
                m_visionBoost = 2;
        }

        internal override void Apply(Character appliedTo)
        {
            appliedTo.Vision += m_visionBoost;
        }

        internal override void Remove(Character removedFrom)
        {
            removedFrom.Vision -= m_visionBoost;
        }

        internal override string Name
        {
            get
            {
                return "Light";
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
                return 2;
            }
        }

        internal override int DefaultEffectLength
        {
            get
            {
                return (new DiceRoll(1, 20, 30)).Roll() * CoreTimingEngine.CTNeededForNewTurn;    //30-50 turns
            }
        }

        #region SaveLoad

        internal override void ReadXml(System.Xml.XmlReader reader)
        {
            m_visionBoost = reader.ReadElementContentAsInt();
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("VisionBoost", m_visionBoost.ToString());
        }

        #endregion
    }
}
