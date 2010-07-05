using System.Collections.Generic;
using System.Xml.Serialization;
using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine.Skills
{
    internal class Skill : ISkill, IXmlSerializable
    {
        public string Name { get; private set; }
        public int Cost { get; private set; }
        public string School { get; private set; }
        public string Description { get; private set; }

        public Dictionary<string, string> Attributes;

        internal Skill(string name, int cost, string school, string description)
        {
            Name = name;
            Cost = cost;
            School = school;
            Description = description;
            Attributes = new Dictionary<string, string>();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("SkillName", Name);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
    }
}
