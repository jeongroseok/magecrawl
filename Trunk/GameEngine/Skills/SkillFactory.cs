using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace Magecrawl.GameEngine.Skills
{
    internal static class SkillFactory
    {
        private static Dictionary<string, Skill> m_skillMapping;

        static SkillFactory()
        {
            LoadMappings();
        }

        public static Skill CreateSkill(string skillName)
        {
            return m_skillMapping[skillName];
        }

        private static void LoadMappings()
        {
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_skillMapping = new Dictionary<string, Skill>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "Skills.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // KeyMappings element
            if (reader.LocalName != "Skills")
            {
                throw new System.InvalidOperationException("Bad skill defination file");
            }

            bool insideSkillNode = false;
            Skill lastSkill = null;
            string attributeName = null;
            string attributeValue = null;
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Skills")
                {
                    break;
                }
                if (reader.LocalName == "Skill")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        insideSkillNode = true;

                        string name = reader.GetAttribute("Name");
                        string school = reader.GetAttribute("School");
                        string description = reader.GetAttribute("Description");

                        string costString = reader.GetAttribute("Cost");
                        int cost = int.Parse(costString);

                        Skill newSkill = new Skill(name, cost, school, description);
                        lastSkill = newSkill;

                        m_skillMapping.Add(name, newSkill);
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (attributeName != null)
                        {
                            lastSkill.Attributes.Add(attributeName, attributeValue);
                            attributeName = null;
                            attributeValue = null;
                        }
                        insideSkillNode = false;
                        lastSkill = null;
                    }
                }
                else if (insideSkillNode)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        attributeName = reader.LocalName;
                    else if (reader.NodeType == XmlNodeType.Text)
                        attributeValue = reader.Value;
                }
            }
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture; 
        }
    }
}
