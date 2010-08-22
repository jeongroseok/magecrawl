using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.StatusEffects.EffectResults
{
    internal class Poison : EffectResult
    {
        private ICharacterCore m_affected;
        private int m_damagePerInterval;
        private bool m_castByPlayer;

        public Poison()
        {
        }

        public Poison(int strength, ICharacterCore caster)
        {
            m_damagePerInterval = strength / 3;
            if (m_damagePerInterval == 0)
                m_damagePerInterval = 1;
            m_castByPlayer = caster is IPlayer;
        }

        internal override void Apply(ICharacterCore appliedTo)
        {
            m_affected = appliedTo;
        }

        internal override void DecreaseCT(int previousCT, int currentCT)
        {
            int CTDifference = previousCT - currentCT;
            int turnsPassed = CTDifference / TimeConstants.CTNeededForNewTurn;
            ICharacterCore casterWasPlayer = m_castByPlayer ? CoreGameEngineInstance.Instance.Player : null;
            for (int i = 0 ; i < turnsPassed ; ++i)
                CoreGameEngineInstance.Instance.DamageTarget(casterWasPlayer, m_damagePerInterval, m_affected);
        }

        internal override string Name
        {
            get 
            {
                return "Poison";
            }
        }

        // Needs to match class name
        internal override string Type
        {
            get
            {
                return "Poison";
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
                return (new DiceRoll(4, 2)).Roll() * TimeConstants.CTNeededForNewTurn;    //4-8 turns
            }
        }

        #region SaveLoad

        internal override void ReadXml(System.Xml.XmlReader reader)
        {
            m_damagePerInterval = reader.ReadElementContentAsInt();
            m_castByPlayer = bool.Parse(reader.ReadElementContentAsString());
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("DamagePerInterval", m_damagePerInterval.ToString());
            writer.WriteElementString("CastByPlayer", m_castByPlayer.ToString());
        }

        #endregion
    }
}
