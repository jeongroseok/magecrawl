using System;
using System.Globalization;
using Magecrawl.Interfaces;
using Magecrawl.Items.Materials;

namespace Magecrawl.Items
{
    internal class Consumable : Item, IConsumable
    {
        private ConsumableEffect m_consumable;

        internal Consumable(string type)
        {
            Attributes["Type"] = type;

            SetupInvokeAttributes();
        }

        internal Consumable(string type, ConsumableEffect effect, int charges, int maxCharges)
        {
            Attributes["Type"] = type;
            m_consumable = effect;
            Attributes["Charges"] = charges.ToString();
            Attributes["MaxCharges"] = maxCharges.ToString();
            
            SetupInvokeAttributes();
            SetupEffectAndStrengthAttributes();
        }

        private void SetupInvokeAttributes()
        {
            Attributes.Add("Invokable", "True");
            switch (Type)
            {
                case "Potion":
                    Attributes.Add("InvokeActionName", "Drink");
                    break;
                case "Scroll":
                    Attributes.Add("InvokeActionName", "Read");
                    break;
                case "Wand":
                    Attributes.Add("InvokeActionName", "Zap");
                    break;
                default:
                    throw new InvalidOperationException("Consumable::ActionVerb don't know how to handle - " + Type);
            }
            Attributes.Add("OnInvokeString", "{0} " + Attributes["InvokeActionName"].ToLower() + "s the {1}.");
        }

        private void SetupEffectAndStrengthAttributes()
        {
            Attributes.Add("InvokeSpellEffect", m_consumable.SpellName);
            Attributes.Add("CasterLevel", m_consumable.CasterLevel.ToString());
        }

        public int Charges 
        {
            get
            {
                return int.Parse(Attributes["Charges"], CultureInfo.InvariantCulture);
            }
        }

        public int MaxCharges 
        {
            get
            {
                return int.Parse(Attributes["MaxCharges"], CultureInfo.InvariantCulture);
            }
        }

        public override string DisplayName
        {
            get 
            {
                return m_consumable.DisplayNames[Type];
            }
        }

        public override string ItemDescription
        {
            get
            {
                return m_consumable.Descriptions[Type];                
            }
        }

        public override string FlavorDescription
        {
            get 
            {
                return "";
            }
        }

        public string Type
        {
            get
            {
                return Attributes["Type"];
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            string effectName = reader.ReadElementContentAsString();
            m_consumable = ItemFactory.Instance.ConsumableEffectFactory.GetEffect(Type, effectName);
            Attributes["Charges"] = reader.ReadElementContentAsString();
            Attributes["MaxCharges"] = reader.ReadElementContentAsString();

            SetupEffectAndStrengthAttributes();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", Type);
            writer.WriteElementString("Effect", m_consumable.EffectName.ToString());
            writer.WriteElementString("Charges", Charges.ToString());
            writer.WriteElementString("MaxCharges", MaxCharges.ToString());
        }
    }
}
