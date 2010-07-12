using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml;
using System.IO;
using Magecrawl.Utilities;

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
            m_baseArmorStats = new Dictionary<string, Dictionary<string, string>>();
            XMLResourceReaderBase.ParseFile("BaseArmorStats.xml", ReadFileCallback);
        }

        void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Armors")
                throw new System.InvalidOperationException("Bad armor stat file");

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
        }
    }
}
