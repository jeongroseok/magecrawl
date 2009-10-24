using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Weapons;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class Player : Character, Interfaces.IPlayer, IXmlSerializable
    {
        private static IWeapon[] m_weaponList = { new MeleeWeapon(), new SimpleBow(), new Spear() };
        private int m_weaponPosition = 0;

        public Player() : base()
        {
        }

        public Player(int x, int y) : base(x, y, 10, 10, 4, "Donblas")
        {
        }

        public override IWeapon CurrentWeapon
        {
            get
            {
                return m_weaponList[m_weaponPosition];
            }
        }

        public void IterateThroughWeapons()
        {
            m_weaponPosition++;
            if (m_weaponPosition >= m_weaponList.Length)
                m_weaponPosition = 0;
        }

        #region SaveLoad

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            base.ReadXml(reader);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Player");
            base.WriteXml(writer);
            writer.WriteEndElement();
        }

        #endregion
    }
}
