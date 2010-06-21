﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using Magecrawl.Interfaces;

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
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_spellMapping = new Dictionary<string, Spell>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "Spells.xml")), settings);
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

                    int range = -1;
                    string rangeString = reader.GetAttribute("Range");
                    if (rangeString != null)
                        range = int.Parse(rangeString);

                    TargetingInfo.TargettingType targettingType = TargetingInfo.TargettingType.Self;
                    string targettingString = reader.GetAttribute("TargettingType");
                    if (targettingString != null)
                        targettingType = (TargetingInfo.TargettingType)Enum.Parse(typeof(TargetingInfo.TargettingType), targettingString);

                    m_spellMapping.Add(name, new Spell(name, school, effectType, cost, targettingType, range));
                }
            }
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture; 
        }
    }
}
