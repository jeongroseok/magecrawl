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

        private WeaponBase m_equipedWeapon;
        private List<Item> m_itemList;

        public Player() : base()
        {
            m_itemList = null;
            m_equipedWeapon = null;
            CurrentMP = 0;
            MaxMP = 0;
        }

        public Player(Point p) : base("Donblas", p, 10, 6)
        {
            m_itemList = new List<Item>();
            m_equipedWeapon = null;
            CurrentMP = 10;
            MaxMP = 10;
        }

        internal IWeapon EquipWeapon(IWeapon weapon)
        {
            if (weapon == null)
                throw new System.ArgumentException("EquipWeapon - Null weapon");
            WeaponBase currentWeapon = weapon as WeaponBase;

            IWeapon oldWeapon = UnequipWeapon();
            
            currentWeapon.Owner = this;
            m_equipedWeapon = currentWeapon;
            return oldWeapon;
        }

        public IWeapon UnequipWeapon()
        {
            if (m_equipedWeapon == null)
                return null;
            WeaponBase oldWeapon = m_equipedWeapon as WeaponBase;
            oldWeapon.Owner = null;
            m_equipedWeapon = null;
            return oldWeapon;
        }

        public override IWeapon CurrentWeapon               
        {
            get
            {
                if (m_equipedWeapon == null)
                    return new MeleeWeapon(this);
                return m_equipedWeapon;
            }
        }

        public IList<ISpell> Spells
        {
            get 
            {
                return new List<ISpell>() 
                {
                    SpellFactory.CreateSpell("Heal"), SpellFactory.CreateSpell("Zap"),
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

            reader.ReadStartElement();
            string equipedWeaponTypeString = reader.ReadElementString();
            if (equipedWeaponTypeString == "None")
            {
                m_equipedWeapon = null;
            }
            else
            {
                Item loadedWeapon = CoreGameEngine.Instance.ItemFactory.CreateItem(equipedWeaponTypeString);
                loadedWeapon.ReadXml(reader);
                WeaponBase weapon = (WeaponBase)loadedWeapon;
                weapon.Owner = this;
                m_equipedWeapon = weapon;
            }
            reader.ReadEndElement();

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

            writer.WriteStartElement("EquipedWeapon");
            Item itemToSave = m_equipedWeapon as Item;
            if (itemToSave != null)
                itemToSave.WriteXml(writer);
            else
                writer.WriteElementString("Type", "None");
            writer.WriteEndElement();

            ListSerialization.WriteListToXML(writer, m_itemList, "Items");
            writer.WriteEndElement();
        }

        #endregion
    }
}
