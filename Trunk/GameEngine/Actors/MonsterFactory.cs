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
using Magecrawl.GameEngine.Actors.MonsterAI;

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
                    string type = reader.GetAttribute("Type");

                    int maxHP = Int32.Parse(reader.GetAttribute("HP"));
                    int vision = Int32.Parse(reader.GetAttribute("Vision"));

                    double evade = double.Parse(reader.GetAttribute("Evade"));
                    
                    double ctIncrease = Double.Parse(reader.GetAttribute("ctIncrease"));
                    double ctMoveCost = Double.Parse(reader.GetAttribute("ctMoveCost"));
                    double ctAttackCost = Double.Parse(reader.GetAttribute("ctAttackCost"));

                    DiceRoll damage = new DiceRoll(reader.GetAttribute("MeleeDamage"));

                    bool intelligent = bool.Parse(reader.GetAttribute("Intelligent"));

                    double ctActCost = 1.0;
                    string actCostString = reader.GetAttribute("ctActCost");
                    if (actCostString != null)
                        ctActCost = Double.Parse(actCostString);

                    Monster newMonster = CreateMonsterCore(type, name, Point.Invalid, maxHP, intelligent, vision, damage, evade, ctIncrease, ctMoveCost, ctActCost, ctAttackCost);                    
                    m_monsterMapping.Add(name, newMonster);
                }
            }
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture; 
        }

        private Monster CreateMonsterCore(string typeName, string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
        {
            List<IMonsterTactic> tactics = new List<IMonsterTactic>();

            switch(typeName)
            {
                case "Bruiser":
                    tactics.Add(new DoubleSwingTactic());
                    tactics.Add(new DefaultTactic());
                    break;
                case "Healer":
                    tactics.Add(new UseFirstAidTactic());
                    tactics.Add(new MoveToWoundedAllyTactic());
                    tactics.Add(new DefaultTactic());
                    break;
                case "Ranged":
                    tactics.Add(new KeepAwayFromMeleeRangeIfAbleTactic());
                    tactics.Add(new UseSlingStoneTactic());
                    tactics.Add(new KeepAwayFromPlayerIfAbleTactic());
                    tactics.Add(new PossiblyRunFromPlayerTactic());
                    tactics.Add(new WaitTactic());
                    break;
                case "Sprinter":
                    tactics.Add(new RushTactic());
                    tactics.Add(new DefaultTactic());
                    break;
                default:
                    tactics.Add(new DefaultTactic());
                    break;
            }

            Monster monster = new Monster(name, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost, tactics);
            return monster;
        }
    }
}
