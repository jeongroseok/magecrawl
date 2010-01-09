using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class MonsterFactory
    {
        private Dictionary<string, Monster> m_monsterMapping;

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

                    int maxHP = Int32.Parse(reader.GetAttribute("HP"));
                    int vision = Int32.Parse(reader.GetAttribute("Vision"));
                    
                    double ctIncrease = Double.Parse(reader.GetAttribute("ctIncrease"));
                    double ctMoveCost = Double.Parse(reader.GetAttribute("ctMoveCost"));
                    double ctAttackCost = Double.Parse(reader.GetAttribute("ctAttackCost"));

                    double ctActCost = 1.0;
                    string actCostString = reader.GetAttribute("ctActCost");
                    if (actCostString != null)
                        ctActCost = Double.Parse(actCostString);

                    m_monsterMapping.Add(name, new Monster(name, Point.Invalid, maxHP, vision, ctIncrease, ctMoveCost, ctActCost, ctAttackCost));
                }
            }
            reader.Close();
        }
    }
}
