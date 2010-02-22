using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class Wand : ItemWithEffects, IWand
    {
        internal Wand(string name, string effectType, int strength, string itemDescription, string flavorText, int maxCharges, DiceRoll newWandPossibleCharges)
            : base(name, effectType, strength, itemDescription, flavorText)
        {
            NewWandPossibleCharges = newWandPossibleCharges;
            Charges = 0;
            MaxCharges = maxCharges;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public int Charges { get; internal set; }
        public int MaxCharges { get; internal set; }

        internal DiceRoll NewWandPossibleCharges { get; set; }

        public override string OnUseString
        {
            get
            {
                return "{0} zaps the {1}.";
            }
        }

        public override List<Magecrawl.GameEngine.Interfaces.ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Zap", true),
                    new ItemOptions("Drop", true)
                };
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            Charges = reader.ReadElementContentAsInt();
            MaxCharges = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("CurrentCharges", Charges.ToString());
            writer.WriteElementString("MaxCharges", MaxCharges.ToString());
        }
    }
}
