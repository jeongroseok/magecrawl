using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class MonsterFactory
    {
        private Dictionary<string, Monster> m_monsterMapping;

        private TCODRandom m_random;

        internal MonsterFactory()
        {
            m_random = new TCODRandom();
            m_monsterMapping = new Dictionary<string, Monster>();
            LoadMappings();
        }

        public Monster CreateMonster(string name)
        {
            return CreateMonster(name, Point.Invalid);
        }

        public Monster CreateMonster(string name, Point p)
        {
            Monster m = (Monster)m_monsterMapping[name].Clone();
            m.Position = p;
            return m;
        }

        public Monster CreateRandomMonster(Point p)
        {
            int targetLocation = m_random.getInt(0, m_monsterMapping.Count - 1);
            string monsterName = m_monsterMapping.Keys.ToList()[targetLocation];
            return CreateMonster(monsterName, p); 
        }

        internal List<Monster> GetAllMonsterListForDebug()
        {
            return m_monsterMapping.Values.ToList();
        }

        private void LoadMappings()
        {
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_monsterMapping = new Dictionary<string, Monster>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "Monsters.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // Items element
            if (reader.LocalName != "Monsters")
            {
                throw new System.InvalidOperationException("Bad monsters file");
            }
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Monsters")
                    break;
                
                if (reader.LocalName == "Monster")
                {
                    string name = reader.GetAttribute("Name");
                    string type = reader.GetAttribute("Type") + "Monster";

                    int maxHP = Int32.Parse(reader.GetAttribute("HP"));
                    int vision = Int32.Parse(reader.GetAttribute("Vision"));

                    double evade = double.Parse(reader.GetAttribute("Evade"));
                    
                    double ctIncrease = Double.Parse(reader.GetAttribute("ctIncrease"));
                    double ctMoveCost = Double.Parse(reader.GetAttribute("ctMoveCost"));
                    double ctAttackCost = Double.Parse(reader.GetAttribute("ctAttackCost"));

                    DiceRoll damage = new DiceRoll(reader.GetAttribute("MeleeDamage"));

                    string primaryWeaponString = reader.GetAttribute("MeleeWeapon");
                    string rangedWeaponString = reader.GetAttribute("RangedWeapon");

                    bool intelligent = bool.Parse(reader.GetAttribute("Intelligent"));

                    double ctActCost = 1.0;
                    string actCostString = reader.GetAttribute("ctActCost");
                    if (actCostString != null)
                        ctActCost = Double.Parse(actCostString);

                    Monster newMonster = CreateMonsterCore(type, name, Point.Invalid, maxHP, intelligent, vision, damage, evade, ctIncrease, ctMoveCost, ctActCost, ctAttackCost);
                    if (primaryWeaponString != null)
                        newMonster.Equip(CoreGameEngine.Instance.ItemFactory.CreateItem(primaryWeaponString));
                    if (rangedWeaponString != null)
                        newMonster.EquipSecondaryWeapon((IWeapon)CoreGameEngine.Instance.ItemFactory.CreateItem(rangedWeaponString));
                    m_monsterMapping.Add(name, newMonster);
                }
            }
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture; 
        }

        private Monster CreateMonsterCore(string typeName, string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
        {
            Assembly weaponsAssembly = this.GetType().Assembly;
            Type type = weaponsAssembly.GetType("Magecrawl.GameEngine.Actors." + typeName);
            if (type != null)
            {
                return Activator.CreateInstance(type, name, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost) as Monster;
            }
            else
            {
                throw new ArgumentException("CreateWeapon - don't know how to make: " + typeName);
            }
        }
    }
}
