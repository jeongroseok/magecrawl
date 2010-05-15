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
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Skills")
                {
                    break;
                }
                if (reader.LocalName == "Skill")
                {
                    string name = reader.GetAttribute("Name");
                    string school = reader.GetAttribute("School");
                    string description = reader.GetAttribute("Description");
                    
                    string costString = reader.GetAttribute("Cost");
                    int cost = int.Parse(costString);

                    string addSpell = reader.GetAttribute("AddSpell");
                    string proficiency = reader.GetAttribute("AddProficiency");

                    Skill newSkill = new Skill(name, cost, school, description);
                    newSkill.AddSpell = addSpell;
                    newSkill.Proficiency = proficiency;
                    m_skillMapping.Add(name, newSkill);
                }
            }
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture; 
        }
    }
}
