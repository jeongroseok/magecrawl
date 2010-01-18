using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Affects;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class Player : Character, Interfaces.IPlayer, IXmlSerializable
    {
        public int CurrentMP { get; internal set; }

        public int MaxMP { get; internal set; }

        private List<Item> m_itemList;

        public Player() : base()
        {
            m_itemList = null;
            CurrentMP = 0;
            MaxMP = 0;
        }

        public Player(Point p) : base((string)Preferences.Instance["PlayerName"], p, 10, 6)
        {
            m_itemList = new List<Item>();
            CurrentMP = 10;
            MaxMP = 10;
            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Sling"));
            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Bronze Spear"));
            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Scroll of Haste"));
            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Wand of Haste"));            
        }

        public IList<ISpell> Spells
        {
            get 
            {
                return new List<ISpell>() 
                {
                    SpellFactory.CreateSpell("Heal"), SpellFactory.CreateSpell("Zap"), SpellFactory.CreateSpell("Lightning Bolt"),
                    SpellFactory.CreateSpell("Haste"), SpellFactory.CreateSpell("False Life"), SpellFactory.CreateSpell("Eagle Eye"),
                    SpellFactory.CreateSpell("Poison Bolt"), SpellFactory.CreateSpell("Poison Touch"),
                    SpellFactory.CreateSpell("Blink"), SpellFactory.CreateSpell("Teleport"), SpellFactory.CreateSpell("Slow")
                };
            }
        }

        public IList<IItem> Items
        {
            get
            {
                return m_itemList.ConvertAll<IItem>(new System.Converter<Item, IItem>(delegate(Item i) { return i as IItem; }));
            }
        }

        public IList<string> StatusEffects
        {
            get
            {
                return m_affects.Select(a => a.Name).ToList();
            }
        }

        internal void TakeItem(Item i)
        {
            m_itemList.Add(i);
        }

        internal void RemoveItem(Item i)
        {
            m_itemList.Remove(i);
        }

        public override DiceRoll MeleeDamage
        {
            get
            {
                return new DiceRoll(1, 1);
            }
        }
        
        public override double MeleeSpeed
        {
            get
            {
                return 1.0;
            }
        }

        #region SaveLoad

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            base.ReadXml(reader);

            CurrentMP = reader.ReadElementContentAsInt();
            MaxMP = reader.ReadElementContentAsInt();

            m_itemList = new List<Item>();
            ReadListFromXMLCore readDelegate = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                Item newItem = CoreGameEngine.Instance.ItemFactory.CreateItem(typeString); 
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

            writer.WriteElementString("CurrentMagic", CurrentMP.ToString());
            writer.WriteElementString("MaxMagic", MaxMP.ToString());

            ListSerialization.WriteListToXML(writer, m_itemList, "Items");
            writer.WriteEndElement();
        }

        #endregion
    }
}
