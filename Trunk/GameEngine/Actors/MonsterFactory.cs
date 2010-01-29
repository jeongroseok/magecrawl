using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class MonsterFactory
    {
        private Dictionary<string, Monster> m_monsterMapping;

        private static TCODRandom m_random;

        static MonsterFactory()
        {
            m_random = new TCODRandom();
        }

        internal MonsterFactory()
        {
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
            int targetLocation = m_random.GetRandomInt(0, m_monsterMapping.Count - 1);
            string monsterName = m_monsterMapping.Keys.ToList()[targetLocation];
            return CreateMonster(monsterName, p); 
        }

        private void LoadMappings()
        {
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

                    double defense = double.Parse(reader.GetAttribute("Defense"));
                    double evade = double.Parse(reader.GetAttribute("Evade"));
                    
                    double ctIncrease = Double.Parse(reader.GetAttribute("ctIncrease"));
                    double ctMoveCost = Double.Parse(reader.GetAttribute("ctMoveCost"));
                    double ctAttackCost = Double.Parse(reader.GetAttribute("ctAttackCost"));

                    DiceRoll damage = new DiceRoll(reader.GetAttribute("MeleeDamage"));

                    string primaryWeaponString = reader.GetAttribute("MeleeWeapon");
                    string rangedWeaponString = reader.GetAttribute("RangedWeapon");

                    double ctActCost = 1.0;
                    string actCostString = reader.GetAttribute("ctActCost");
                    if (actCostString != null)
                        ctActCost = Double.Parse(actCostString);

                    Monster newMonster = CreateMonsterCore(type, name, Point.Invalid, maxHP, vision, damage, defense, evade, ctIncrease, ctMoveCost, ctActCost, ctAttackCost);
                    if (primaryWeaponString != null)
                        newMonster.Equip(CoreGameEngine.Instance.ItemFactory.CreateItem(primaryWeaponString));
                    if (rangedWeaponString != null)
                        newMonster.EquipSecondaryWeapon((IWeapon)CoreGameEngine.Instance.ItemFactory.CreateItem(rangedWeaponString));
                    m_monsterMapping.Add(name, newMonster);
                }
            }
            reader.Close();
        }

        private Monster CreateMonsterCore(string typeName, string name, Point p, int maxHP, int vision, DiceRoll damage, double defense, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
        {
            Assembly weaponsAssembly = this.GetType().Assembly;
            Type type = weaponsAssembly.GetType("Magecrawl.GameEngine.Actors." + typeName);
            if (type != null)
            {
                return Activator.CreateInstance(type, name, p, maxHP, vision, damage, defense, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost) as Monster;
            }
            else
            {
                throw new ArgumentException("CreateWeapon - don't know how to make: " + typeName);
            }
        }
    }
}
