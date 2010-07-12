using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public class SpellGraphicsInfo
    {
        public static SpellGraphicsInfo Instance = new SpellGraphicsInfo();

        private Dictionary<string, string> m_spellAttributes;

        private SpellGraphicsInfo()
        {
            LoadSpellAttributes();
        }

        public bool DrawLastFrame(ISpell spell)
        {
            return m_spellAttributes.ContainsKey(spell.Name) && m_spellAttributes[spell.Name].Contains("DrawLastFrame");
        }

        public int GetTailLength(ISpell spell)
        {
            if (m_spellAttributes.ContainsKey(spell.Name))
            {
                string attributeString = m_spellAttributes[spell.Name];
                if (attributeString.Contains("Tail"))
                {
                    int tailLocation = attributeString.IndexOf("Tail=");
                    return (int)char.GetNumericValue(attributeString[tailLocation + 5]);
                }
            }
            return 1;
        }

        public TCODColor GetColorOfSpellFromSchool(string schoolName)
        {
            switch (schoolName)
            {
                case "Light":
                    return ColorPresets.Wheat;
                case "Darkness":
                    return ColorPresets.DarkGray;
                case "Fire":
                    return ColorPresets.Firebrick;
                case "Arcane":
                    return ColorPresets.DarkViolet;
                case "Air":
                    return ColorPresets.LightBlue;
                case "Earth":
                    return ColorPresets.SaddleBrown;
                case "Water":
                    return ColorPresets.SteelBlue;
                default:
                    return ColorPresets.White;
            }
        }

        private void LoadSpellAttributes()
        {
            m_spellAttributes = new Dictionary<string, string>();
            XMLResourceReaderBase.ParseFile("Spells.xml", ReadFileCallback);
        }

        private void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Spells")
                throw new System.InvalidOperationException("Bad spells file");

            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Spells")
                    break;

                if (reader.LocalName == "Spell" && reader.NodeType == XmlNodeType.Element)
                {
                    string name = reader.GetAttribute("Name");
                    string attributes = reader.GetAttribute("DrawingAttributes");

                    if (attributes != null)
                        m_spellAttributes.Add(name, attributes);
                }
            }
        }
    }
}
