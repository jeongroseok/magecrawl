using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public static class SpellGraphicsInfo
    {
        private static Dictionary<string, string> m_spellAttributes;

        static SpellGraphicsInfo()
        {
            LoadSpellAttributes();
        }

        public static bool DrawLastFrame(ISpell spell)
        {
            return m_spellAttributes.ContainsKey(spell.Name) && m_spellAttributes[spell.Name].Contains("DrawLastFrame");
        }

        public static int GetTailLength(ISpell spell)
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

        public static TCODColor GetColorOfSpellFromSchool(string schoolName)
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

        private static void LoadSpellAttributes()
        {
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_spellAttributes = new Dictionary<string, string>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "Spells.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // Items element
            if (reader.LocalName != "Spells")
            {
                throw new System.InvalidOperationException("Bad spells file");
            }
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Spells")
                    break;

                if (reader.LocalName == "Spell")
                {
                    string name = reader.GetAttribute("Name");
                    string attributes = reader.GetAttribute("DrawingAttributes");

                    if (attributes != null)
                        m_spellAttributes.Add(name, attributes);
                }
            }
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture;
        }
    }
}
