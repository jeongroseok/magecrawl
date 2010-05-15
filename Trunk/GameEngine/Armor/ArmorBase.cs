using System.Collections.Generic;
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
            Summoned = false;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool Summoned
        {
            get;
            set;
        }

        protected bool IsUnequipable(IArmor armor)
        {
            return !IsSummoned(armor);
        }

        protected bool IsSummoned(IArmor armor)
        {
            if (armor == null)
                return true;
            return ((ArmorBase)armor).Summoned;
        }

        protected List<ItemOptions> PlayerOptionsInternal(IArmor armor)
        {
            List<ItemOptions> optionList = new List<ItemOptions>();

            if (armor == this)
            {
                if (IsUnequipable(armor))
                    optionList.Add(new ItemOptions("Unequip", true));
                if (IsSummoned(armor))
                    optionList.Add(new ItemOptions("Dismiss Spell", true));
            }
            else
            {
                if (IsUnequipable(armor))
                    optionList.Add(new ItemOptions("Equip", true));
                optionList.Add(new ItemOptions("Drop", true));
            }

            return optionList;
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
            Summoned = bool.Parse(reader.ReadElementContentAsString());
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("Summoned", Summoned.ToString());
        }
    }
}
