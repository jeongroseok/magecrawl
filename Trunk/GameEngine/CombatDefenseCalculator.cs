using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine
{
    internal static class CombatDefenseCalculator
    {
        private static Dictionary<string, double> m_combatConstants;
        private const double BaseEvade = 15;

        static CombatDefenseCalculator()
        {
            m_combatConstants = new Dictionary<string, double>();
            LoadSettings();
        }

        private static void LoadSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "CombatConstants.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // KeyMappings element
            if (reader.LocalName != "CombatConstants")
            {
                // We didn't find our preference file...
                throw new System.IO.FileNotFoundException();
            }
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // If we're at an end element, if its the preference section, break, else skip it
                    if (reader.LocalName == "CombatConstants")
                        break;
                    else
                        continue;
                }
                else
                {
                    string name = reader.LocalName;
                    reader.Read();
                    m_combatConstants[name] = double.Parse(reader.Value);
                }
            }
            reader.Close();
        }

        public static double CalculateDefense(ICharacter character)
        {
            IPlayer player = character as IPlayer;
            if (player == null)
                throw new NotImplementedException("Monsters don't use armor for defense yet");

            return GetArmorList(player).Sum(x => x.Defense);
        }

        public static double CalculateEvade(ICharacter character)
        {
            IPlayer player = character as IPlayer;
            if (player == null)
                throw new NotImplementedException("Monsters don't use armor for defense yet");

            double evadeBeforeArmorRestrictions = m_combatConstants["CharacterEvadeBase"] + GetArmorList(player).Sum(x => x.Evade);

            return Math.Min(evadeBeforeArmorRestrictions, GetEvadeCap(GetTotalArmorWeight(player)));
        }

        private static IEnumerable<IArmor> GetArmorList(IPlayer player)
        {
            List<IArmor> armorList = new List<IArmor>() { player.ChestArmor, player.Gloves, player.Boots, player.Headpiece};
            return armorList.Where(x => x != null);
        }

        private static ArmorWeight GetTotalArmorWeight(IPlayer player)
        {
            ArmorWeight largestWeight = ArmorWeight.None;
            foreach (IArmor a in GetArmorList(player))
            {
                if (a.Weight > largestWeight)
                    largestWeight = a.Weight;
            }
            return largestWeight;
        }

        private static double GetEvadeCap(ArmorWeight weight)
        {
            switch (weight)
            {
                case ArmorWeight.None:
                    return m_combatConstants["NoArmorEvadeCap"];
                case ArmorWeight.Light:
                    return m_combatConstants["LightArmorEvadeCap"];
                case ArmorWeight.Standard:
                    return m_combatConstants["StandardArmorEvadeCap"];
                case ArmorWeight.Heavy:
                    return m_combatConstants["HeavyArmorEvadeCap"];
                default:
                    throw new InvalidOperationException("GetEvadeCap - Invalid weight?");
            }
        }
    }
}
