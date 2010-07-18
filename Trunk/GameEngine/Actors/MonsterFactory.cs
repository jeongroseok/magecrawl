using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using libtcod;
using Magecrawl.GameEngine.Actors.MonsterAI;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class MonsterFactory
    {
        private Dictionary<string, Dictionary<string, string>> m_monsterStats;
        private Dictionary<string, List<Pair<string, int>>> m_monsterInstances;

        private TCODRandom m_random;

        internal MonsterFactory()
        {
            m_random = new TCODRandom();
            LoadMappings();
        }

        public Monster CreateMonster(string type, int level)
        {
            return CreateMonster(type, level, Point.Invalid);
        }

        public Monster CreateRandomMonster(int level, Point p)
        {
            string baseType = m_monsterStats.Keys.ToList().Randomize()[0];
            return CreateMonster(baseType, level, p);
        }

        public Monster CreateMonster(string baseType, int level, Point p)
        {
            var monsterInstance = m_monsterInstances[baseType].Find(x => x.Second == level);
            if (monsterInstance == null)
                throw new InvalidOperationException("Unable to create " + baseType + " of level " + level.ToString() + ".");
            string monsterName = monsterInstance.First;

            string AIType = m_monsterStats[baseType]["AIType"];
            int baseHP = int.Parse(m_monsterStats[baseType]["BaseHP"], CultureInfo.InvariantCulture);
            int hpPerLevel = int.Parse(m_monsterStats[baseType]["HPPerLevel"], CultureInfo.InvariantCulture);
            int maxHP = baseHP + (level * hpPerLevel);
            bool intelligent = bool.Parse(m_monsterStats[baseType]["Intelligent"]);
            int vision = int.Parse(m_monsterStats[baseType]["BaseVision"], CultureInfo.InvariantCulture);
            string baseDamageString = m_monsterStats[baseType]["BaseDamage"];
            DiceRoll damagePerLevel = new DiceRoll(m_monsterStats[baseType]["DamagePerLevel"]);
            DiceRoll damage = new DiceRoll(baseDamageString);
            for (int i = 0; i < level; ++i)
                damage.Add(damagePerLevel);
            int evade = int.Parse(m_monsterStats[baseType]["BaseEvade"], CultureInfo.InvariantCulture);
            double ctIncreaseModifer = double.Parse(m_monsterStats[baseType]["BaseCTIncrease"], CultureInfo.InvariantCulture);
            double ctMoveCost = double.Parse(m_monsterStats[baseType]["BaseCTMoveCost"], CultureInfo.InvariantCulture);
            double ctActCost = double.Parse(m_monsterStats[baseType]["BaseCTActCost"], CultureInfo.InvariantCulture);
            double ctAttackCost = double.Parse(m_monsterStats[baseType]["BaseCTAttackCost"], CultureInfo.InvariantCulture);
            return CreateMonsterCore(baseType, AIType, monsterName, level, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost);
        }

        internal List<INamedItem> GetAllMonsterListForDebug()
        {
            List<INamedItem> returnList = new List<INamedItem>();
            foreach (string s in m_monsterStats.Keys)
                returnList.Add(new TextElement(s));
            return returnList;
        }

        private void LoadMappings()
        {
            m_monsterStats = new Dictionary<string, Dictionary<string, string>>();
            m_monsterInstances = new Dictionary<string, List<Pair<string, int>>>();

            XMLResourceReaderBase.ParseFile("Monsters.xml", ReadFileCallback);
        }

        private void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Monsters")
                throw new System.InvalidOperationException("Bad monsters file");

            string lastBaseName = "";
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Monsters")
                    break;

                if (reader.LocalName == "Monster" && reader.NodeType == XmlNodeType.Element)
                {
                    lastBaseName = reader.GetAttribute("BaseName");
                    m_monsterStats[lastBaseName] = new Dictionary<string, string>();
                    m_monsterInstances[lastBaseName] = new List<Pair<string, int>>();
                    m_monsterStats[lastBaseName]["AIType"] = reader.GetAttribute("AIType");
                    m_monsterStats[lastBaseName]["Intelligent"] = reader.GetAttribute("Intelligent");
                }
                else if (reader.LocalName == "BaseStats" && reader.NodeType == XmlNodeType.Element)
                {
                    m_monsterStats[lastBaseName]["BaseLevel"] = reader.GetAttribute("BaseLevel");
                    m_monsterStats[lastBaseName]["BaseVision"] = reader.GetAttribute("BaseVision");
                    m_monsterStats[lastBaseName]["BaseHP"] = reader.GetAttribute("BaseHP");
                    m_monsterStats[lastBaseName]["BaseDefence"] = reader.GetAttribute("BaseDefence");
                    m_monsterStats[lastBaseName]["BaseEvade"] = reader.GetAttribute("BaseEvade");
                    m_monsterStats[lastBaseName]["BaseDamage"] = reader.GetAttribute("BaseDamage");
                    m_monsterStats[lastBaseName]["BaseCTIncrease"] = reader.GetAttribute("BaseCTIncrease");
                    m_monsterStats[lastBaseName]["BaseCTMoveCost"] = reader.GetAttribute("BaseCTMoveCost");
                    m_monsterStats[lastBaseName]["BaseCTAttackCost"] = reader.GetAttribute("BaseCTAttackCost");
                    if (reader.GetAttribute("BaseCTActCost") != null)
                        m_monsterStats[lastBaseName]["BaseCTActCost"] = reader.GetAttribute("BaseCTActCost");
                    else
                        m_monsterStats[lastBaseName]["BaseCTActCost"] = "1.0";
                }
                else if (reader.LocalName == "StatsPerLevel" && reader.NodeType == XmlNodeType.Element)
                {
                    m_monsterStats[lastBaseName]["DamagePerLevel"] = reader.GetAttribute("DamagePerLevel");
                    m_monsterStats[lastBaseName]["HPPerLevel"] = reader.GetAttribute("HPPerLevel");
                }
                else if (reader.LocalName == "MonsterInstance" && reader.NodeType == XmlNodeType.Element)
                {
                    m_monsterInstances[lastBaseName].Add(new Pair<string, int>(reader.GetAttribute("Name"), int.Parse(reader.GetAttribute("Level"))));
                }
            }
        }

        private Monster CreateMonsterCore(string baseType, string AIType, string name, int level, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
        {
            List<IMonsterTactic> tactics = new List<IMonsterTactic>();

            switch (AIType)
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

            Monster monster = new Monster(baseType, name, level, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost, tactics);
            return monster;
        }
    }
}
