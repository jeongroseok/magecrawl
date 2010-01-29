using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;

namespace Magecrawl.GameEngine.Armor
{
    internal abstract class ArmorBase : ItemWithOwner, IArmor
    {
        internal ArmorBase(string name, ArmorWeight weight, double defense, double evade, string itemDescription, string flavorText)
            : base(null, name, itemDescription, flavorText)
        {
            Weight = weight;
            Defense = defense;
            Evade = evade;
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
            get;
            private set;
        }

        public double Defense
        {
            get;
            private set;
        }

        public double Evade
        {
            get;
            private set;
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
