using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml;
using System.IO;

namespace Magecrawl.Items
{
    internal class BaseArmorStats
    {
        public static BaseArmorStats Instance = new BaseArmorStats();

        private Dictionary<string, Dictionary<string, string>> m_baseArmorStats;

        private BaseArmorStats()
        {
            LoadMappings();
        }

        public void LoadMappingIntoAttributes(Armor armor)
        {
            Dictionary<string, string> specificDictionary = m_baseArmorStats[armor.Type];
            foreach (string key in specificDictionary.Keys)
                armor.Attributes[key] = specificDictionary[key];
        }

        private void LoadMappings()
        {
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_baseArmorStats = new Dictionary<string, Dictionary<string, string>>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "BaseArmorStats.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // Items element
            if (reader.LocalName != "Armors")
            {
                throw new System.InvalidOperationException("Bad armor stat file");
            }

            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Armors")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Armor")
                {
                    string name = reader.GetAttribute("Name");
                    m_baseArmorStats[name] = new Dictionary<string, string>();
                    m_baseArmorStats[name]["BaseStaminaBonus"] = reader.GetAttribute("BaseStaminaBonus");
                }
            }
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture;
        }
    }
}
