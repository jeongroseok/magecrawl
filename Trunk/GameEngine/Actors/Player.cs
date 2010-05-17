using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Armor;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.GameEngine.Skills;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class Player : Character, IPlayer, IXmlSerializable
    {
        public IArmor ChestArmor { get; internal set; }
        public IArmor Headpiece { get; internal set; }
        public IArmor Gloves { get; internal set; }
        public IArmor Boots { get; internal set; }

        private List<Item> m_itemList;
        private List<Skill> m_skills;

        public int LastTurnSeenAMonster { get; set; }

        public Player() : base()
        {
            m_itemList = null;
            m_skills = null;
            CurrentMP = 0;
            MaxMP = 0;
            LastTurnSeenAMonster = 0;
        }

        public Player(string name, Point p) : base(name, p, 12, 6)
        {
            m_itemList = new List<Item>();
            m_skills = new List<Skill>();
            CurrentMP = 10;
            MaxMP = 10;
            LastTurnSeenAMonster = 0;

            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Minor Health Potion"));
            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Minor Health Potion"));
            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Minor Mana Potion"));            
            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Wand Of Magic Missile"));
            m_itemList.Add(CoreGameEngine.Instance.ItemFactory.CreateItem("Wand Of Sparks"));            
            Equip(CoreGameEngine.Instance.ItemFactory.CreateItem("Wooden Cudgel"));
            Equip(CoreGameEngine.Instance.ItemFactory.CreateItem("Robe"));
            Equip(CoreGameEngine.Instance.ItemFactory.CreateItem("Wool Cap"));
            Equip(CoreGameEngine.Instance.ItemFactory.CreateItem("Sandles"));
            Equip(CoreGameEngine.Instance.ItemFactory.CreateItem("Wool Gloves"));
        }

        private int m_baseCurrentHP;
        public override int CurrentHP 
        {
            get
            {
                return m_baseCurrentHP;
            }
            internal set
            {
                m_baseCurrentHP = value;
            }
        }

        private int m_baseMaxHP;
        public override int MaxHP
        {
            get
            {
                int baseMax = m_baseMaxHP;

                int percentageBonus = 0;
                foreach (Skill s in Skills)
                {
                    percentageBonus += s.HPBonus;
                }
                baseMax = (int)(baseMax * (1.0 + ((float)percentageBonus / 100.0f)));

                return baseMax;
            }
            internal set
            {
                m_baseMaxHP = value;
            }
        }

        private int m_baseCurrentMP;
        public int CurrentMP 
        {
            get
            {
                return m_baseCurrentMP;
            }
            internal set
            {
                m_baseCurrentMP = value;
            }
        }

        private int m_baseMaxMP;
        public int MaxMP 
        {
            get
            {
                int baseMax = m_baseMaxMP;

                int percentageBonus = 0;
                foreach (Skill s in Skills)
                {
                    percentageBonus += s.MPBonus;
                }
                baseMax = (int)(baseMax * (1.0 + ((float)percentageBonus / 100.0f)));

                return baseMax;
            }
            internal set
            {
                m_baseMaxMP = value;
            }
        }

        public IEnumerable<ISpell> Spells
        {
            get 
            {
                List<ISpell> returnList = new List<ISpell>();
                foreach (Skill skill in Skills)
                {
                    if (skill.NewSpell)
                        returnList.Add(SpellFactory.CreateSpell(skill.AddSpell));
                }
                return returnList;
            }
        }

        public int SpellStrength(string spellType)
        {
            return CalculateSpellStrengthFromPassiveSkills(spellType);
        }

        private int CalculateSpellStrengthFromPassiveSkills(string typeName)
        {
            int strength = 1;
            foreach (Skill s in Skills)
            {
                if (s.Proficiency == typeName)
                    strength++;
            }
            return strength;
        }

        // Returns amount actually healed by
        public int HealMP(int toHeal)
        {
            int previousMP = CurrentMP;
            CurrentMP = Math.Min(CurrentMP + toHeal, MaxMP);
            return CurrentMP - previousMP;
        }

        public IEnumerable<IItem> Items
        {
            get
            {
                return m_itemList.ConvertAll<IItem>(i => i).ToList();
            }
        }

        public IEnumerable<string> StatusEffects
        {
            get
            {
                return m_affects.Select(a => a.Name).ToList();
            }
        }

        public IEnumerable<ISkill> Skills
        {
            get
            {
                return m_skills.ConvertAll<ISkill>(x => x).ToList();
            }
        }

        public void AddSkill(ISkill skill)
        {
             m_skills.Add((Skill)skill);
        }

        internal override IItem Equip(IItem item)
        {
            if (item is ChestArmor)
            {
                IItem previousArmor = ChestArmor;
                ChestArmor = (IArmor)item;
                return previousArmor;
            }
            if (item is Headpiece)
            {
                IItem previousArmor = Headpiece;
                Headpiece = (IArmor)item;
                return previousArmor;
            }
            if (item is Gloves)
            {
                IItem previousArmor = Gloves;
                Gloves = (IArmor)item;
                return previousArmor;
            }
            if (item is Boots)
            {
                IItem previousArmor = Boots;
                Boots = (IArmor)item;
                return previousArmor;
            }

            return base.Equip(item);
        }

        internal override IItem Unequip(IItem item)
        {
            if (item is ChestArmor)
            {
                IItem previousArmor = ChestArmor;
                ChestArmor = null;
                return previousArmor;
            }
            if (item is Headpiece)
            {
                IItem previousArmor = Headpiece;
                Headpiece = null;
                return previousArmor;
            }
            if (item is Gloves)
            {
                IItem previousArmor = Gloves;
                Gloves = null;
                return previousArmor;
            }
            if (item is Boots)
            {
                IItem previousArmor = Boots;
                Boots = null;
                return previousArmor;
            }

            return base.Unequip(item);
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
                return new DiceRoll(1, 2);
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

            m_baseCurrentHP = reader.ReadElementContentAsInt();
            m_baseMaxHP = reader.ReadElementContentAsInt();

            m_baseCurrentMP = reader.ReadElementContentAsInt();
            m_baseMaxMP = reader.ReadElementContentAsInt();

            LastTurnSeenAMonster = reader.ReadElementContentAsInt();

            ChestArmor = (IArmor)Item.ReadXmlEntireNode(reader, this);
            Headpiece = (IArmor)Item.ReadXmlEntireNode(reader, this);
            Gloves = (IArmor)Item.ReadXmlEntireNode(reader, this);
            Boots = (IArmor)Item.ReadXmlEntireNode(reader, this);

            m_itemList = new List<Item>();
            ReadListFromXMLCore readItemDelegate = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                Item newItem = CoreGameEngine.Instance.ItemFactory.CreateItem(typeString); 
                newItem.ReadXml(reader);
                m_itemList.Add(newItem);
            });
            ListSerialization.ReadListFromXML(reader, readItemDelegate);

            m_skills = new List<Skill>();
            ReadListFromXMLCore readSkillDelegate = new ReadListFromXMLCore(delegate
            {
                string skillName = reader.ReadElementContentAsString();
                m_skills.Add(SkillFactory.CreateSkill(skillName));
            });
            ListSerialization.ReadListFromXML(reader, readSkillDelegate);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Player");
            base.WriteXml(writer);

            writer.WriteElementString("BaseCurrentHP", m_baseCurrentHP.ToString());
            writer.WriteElementString("BaseMaxHP", m_baseMaxHP.ToString());

            writer.WriteElementString("BaseCurrentMagic", m_baseCurrentMP.ToString());
            writer.WriteElementString("BaseMaxMagic", m_baseMaxMP.ToString());

            writer.WriteElementString("LastTurnSeenAMonster", LastTurnSeenAMonster.ToString());

            Item.WriteXmlEntireNode((Item)ChestArmor, "ChestArmor", writer);
            Item.WriteXmlEntireNode((Item)Headpiece, "Headpiece", writer);
            Item.WriteXmlEntireNode((Item)Gloves, "Gloves", writer);
            Item.WriteXmlEntireNode((Item)Boots, "Boots", writer);

            ListSerialization.WriteListToXML(writer, m_itemList, "Items");
            ListSerialization.WriteListToXML(writer, m_skills, "Skills");

            writer.WriteEndElement();
        }

        #endregion
    }
}
