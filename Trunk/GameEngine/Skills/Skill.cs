using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Skills
{
    internal class Skill : ISkill, IXmlSerializable
    {
        public string Name { get; private set; }
        public int Cost { get; private set; }
        public string School { get; private set; }
        public string Description { get; private set; }

        internal int HPBonus { get; set; }
        internal int MPBonus { get; set; }
        internal string Proficiency { get; set; }
        internal string AddSpell { get; set; }
        internal string ArmorSkill { get; set; }

        internal Skill(string name, int cost, string school, string description)
        {
            Name = name;
            Cost = cost;
            School = school;
            Description = description;
            AddSpell = null;
            Proficiency = null;
            HPBonus = 0;
            MPBonus = 0;
        }

        public bool NewSpell
        {
            get
            {
                return AddSpell != null;
            }
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
