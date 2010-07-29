using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Magic
{
    internal class SpellFactory
    {
        public static SpellFactory Instance = new SpellFactory();

        private Dictionary<string, Spell> m_spellMapping;

        private SpellFactory()
        {
            LoadMappings();
        }

        public Spell CreateSpell(string spellName)
        {
            return m_spellMapping[spellName];
        }

        private void LoadMappings()
        {
            m_spellMapping = new Dictionary<string, Spell>();

            XMLResourceReaderBase.ParseFile("Spells.xml", ReadFileCallback);
        }

        private void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Spells")
                throw new System.InvalidOperationException("Bad spell defination file");

            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Spells")
                {
                    break;
                }
                if (reader.LocalName == "Spell")
                {
                    string name = reader.GetAttribute("Name");
                    string school = reader.GetAttribute("School");
                    string effectType = reader.GetAttribute("EffectType");

                    string costString = reader.GetAttribute("Cost");
                    int cost = int.Parse(costString);
                    
                    DiceRoll baseDamage = DiceRoll.Invalid;
                    string baseDamageString = reader.GetAttribute("BaseDamageString");
                    if (baseDamageString != null)
                        baseDamage = new DiceRoll(baseDamageString);

                    DiceRoll damagePerLevel = DiceRoll.Invalid;
                    string damagePerLevelString = reader.GetAttribute("DamagePerLevel");
                    if (damagePerLevelString != null)
                        damagePerLevel = new DiceRoll(damagePerLevelString);

                    int range = -1;
                    string rangeString = reader.GetAttribute("Range");
                    if (rangeString != null)
                        range = int.Parse(rangeString);

                    TargetingInfo.TargettingType targettingType = TargetingInfo.TargettingType.Self;
                    string targettingString = reader.GetAttribute("TargettingType");
                    if (targettingString != null)
                        targettingType = (TargetingInfo.TargettingType)Enum.Parse(typeof(TargetingInfo.TargettingType), targettingString);

                    m_spellMapping.Add(name, new Spell(name, school, effectType, cost, targettingType, range, baseDamage, damagePerLevel));
                }
            }
        }
    }
}
