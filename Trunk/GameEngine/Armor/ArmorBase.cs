using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;

namespace Magecrawl.GameEngine.Armor
{
    internal abstract class ArmorBase : ItemWithOwner, IArmor
    {
        ArmorWeight m_weight;
        internal ArmorBase(string name, ArmorWeight weight, string itemDescription, string flavorText)
            : base(null, name, itemDescription, flavorText)
        {
            m_weight = weight;
            CanNotUnequip = false;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool CanNotUnequip
        {
            get;
            set;
        }

        protected bool IsUnequipable(IArmor armor)
        {
            if (armor == null)
                return true;
            return !((ArmorBase)armor).CanNotUnequip;
        }

        public ArmorWeight Weight
        {
            get
            {
                return m_weight;
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            CanNotUnequip = bool.Parse(reader.ReadElementContentAsString());
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("CanNotUnequip", CanNotUnequip.ToString());
        }
    }
}
