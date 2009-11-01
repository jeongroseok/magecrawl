using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Magic
{
    internal static class SpellFactory
    {
        private static Dictionary<string, Spell> m_spellMapping;

        static SpellFactory()
        {
            LoadMappings();
        }

        public static Spell CreateSpell(string spellName)
        {
            return m_spellMapping[spellName];
        }

        private static void LoadMappings()
        {
            m_spellMapping = new Dictionary<string, Spell>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader("Spells.xml"), settings);
            reader.Read();  // XML declaration
            reader.Read();  // KeyMappings element
            if (reader.LocalName != "Spells")
            {
                throw new System.InvalidOperationException("Bad spell defination file");
            }
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

                    string strengthString = reader.GetAttribute("Strength");
                    int strength = int.Parse(strengthString);

                    string targetType = reader.GetAttribute("TargetType");

                    m_spellMapping.Add(name, new Spell(name, school, effectType, cost, strength, targetType));
                }
            }
            reader.Close();
        }
    }
}
