using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Skills
{
    internal class SkillFactory
    {
        public static readonly SkillFactory Instance = new SkillFactory();

        private static Dictionary<string, Skill> m_skillMapping;

        private SkillFactory()
        {
            LoadMappings();
        }

        public Skill CreateSkill(string skillName)
        {
            return m_skillMapping[skillName];
        }

        public IEnumerable<Skill> GetAllSkills()
        {
            return m_skillMapping.Values;
        }

        private void LoadMappings()
        {
            m_skillMapping = new Dictionary<string, Skill>();
            XMLResourceReaderBase.ParseFile("Skills.xml", ReadFileCallback);
        }

        private void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Skills")
                throw new System.InvalidOperationException("Bad skill defination file");

            bool insideSkillNode = false;
            Skill lastSkill = null;
            string attributeName = null;
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Skills")
                    break;
                
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
                        insideSkillNode = false;
                        lastSkill = null;
                    }
                }
                else if (insideSkillNode)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        attributeName = reader.Name;
                        lastSkill.Attributes[attributeName] = null;
                    }
                    else if (reader.NodeType == XmlNodeType.Text)
                    {
                        lastSkill.Attributes[attributeName] = reader.Value;
                        attributeName = null;
                    }
                }
            }
        }
    }
}
