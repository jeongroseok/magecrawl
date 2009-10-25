using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.SaveLoad;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class Player : Character, Interfaces.IPlayer, IXmlSerializable
    {
        private IWeapon[] m_weaponList;
        private int m_weaponPosition = 0;
        private List<Item> m_itemList;

        public Player() : base()
        {
            m_weaponList = new IWeapon[4];
            m_weaponList[0] = new MeleeWeapon(this);
            m_weaponList[1] = new SimpleBow(this);
            m_weaponList[2] = new Spear(this);
            m_weaponList[3] = new Sword(this);
            m_itemList = null;
        }

        public Player(int x, int y) : base(x, y, 10, 10, 6, 10, 10, "Donblas")
        {
            m_weaponList = new IWeapon[4];
            m_weaponList[0] = new MeleeWeapon(this);
            m_weaponList[1] = new SimpleBow(this);
            m_weaponList[2] = new Spear(this);
            m_weaponList[3] = new Sword(this);
            m_itemList = new List<Item>();
        }

        public override IWeapon CurrentWeapon               
        {
            get
            {
                return m_weaponList[m_weaponPosition];
            }
        }

        public IList<IItem> Items
        {
            get
            {
                return m_itemList.ConvertAll<IItem>(new System.Converter<Item, IItem>(delegate(Item i) { return i as IItem; }));
            }
        }

        public void IterateThroughWeapons()
        {
            m_weaponPosition++;
            if (m_weaponPosition >= m_weaponList.Length)
                m_weaponPosition = 0;
        }

        internal void TakeItem(Item i)
        {
            m_itemList.Add(i);
        }

        #region SaveLoad

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            base.ReadXml(reader);

            m_itemList = new List<Item>();
            ReadListFromXMLCore readDelegate = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                Item newItem = Item.CreateItemObjectFromTypeString(typeString);
                newItem.ReadXml(reader);
                m_itemList.Add(newItem);
            });
            ListSerialization.ReadListFromXML(reader, readDelegate);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Player");
            base.WriteXml(writer);
            ListSerialization.WriteListToXML(writer, m_itemList, "Items");
            writer.WriteEndElement();
        }

        #endregion
    }
}
