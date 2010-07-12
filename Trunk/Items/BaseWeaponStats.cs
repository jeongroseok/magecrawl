using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml;
using System.IO;
using Magecrawl.Utilities;

namespace Magecrawl.Items
{
    internal class BaseWeaponStats
    {
        public static BaseWeaponStats Instance = new BaseWeaponStats();

        private Dictionary<string, Dictionary<string, string>> m_baseWeaponStats;

        private BaseWeaponStats()
        {
            LoadMappings();
        }

        public void LoadMappingIntoAttributes(StatsBasedWeapon weapon)
        {
            Dictionary<string, string> specificDictionary = m_baseWeaponStats[weapon.Type];
            foreach (string key in specificDictionary.Keys)
                weapon.Attributes[key] = specificDictionary[key];
        }

        private void LoadMappings()
        {
            m_baseWeaponStats = new Dictionary<string, Dictionary<string, string>>();
            XMLResourceReaderBase.ParseFile("BaseWeaponStats.xml", ReadFileCallback);
        }

        void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Weapons")
                throw new System.InvalidOperationException("Bad weapon stat file");

            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Weapons")
                    break;

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Weapon")
                {
                    string name = reader.GetAttribute("Name");
                    m_baseWeaponStats[name] = new Dictionary<string, string>();
                    m_baseWeaponStats[name]["Name"] = name;
                    m_baseWeaponStats[name]["BaseDamage"] = reader.GetAttribute("BaseDamage");
                    m_baseWeaponStats[name]["BaseSpeed"] = reader.GetAttribute("BaseSpeed");

                    string baseRange = reader.GetAttribute("BaseRange");
                    if (baseRange != null)
                        m_baseWeaponStats[name]["BaseRange"] = baseRange;

                    string baseMinRange = reader.GetAttribute("BaseMinRange");
                    if (baseMinRange != null)
                        m_baseWeaponStats[name]["BaseMinRange"] = baseMinRange;

                    string baseFalloffStart = reader.GetAttribute("BaseFalloffStart");
                    if (baseFalloffStart != null)
                        m_baseWeaponStats[name]["BaseFalloffStart"] = baseFalloffStart;

                    string falloffAmount = reader.GetAttribute("FalloffAmount");
                    if (falloffAmount != null)
                        m_baseWeaponStats[name]["FalloffAmount"] = falloffAmount;
                }
            }
        }
    }
}
