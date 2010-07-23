using System;
using System.Globalization;
using System.Xml;
using Magecrawl.Interfaces;
using Magecrawl.Items.Materials;

namespace Magecrawl.Items
{
    internal class Armor : Item, IArmor
    {
        private Material m_material;
        private Quality m_quality;

        private double m_evade;
        private int m_staminaBonus;

        internal Armor(string type)
        {
            Attributes["Type"] = type;
        }

        internal Armor(string type, Material material, Quality quality)
        {
            Attributes["Type"] = type;
            m_material = material;
            m_quality = quality;
            Calculate();
        }

        public override string DisplayName
        {
            get 
            {
                return m_material.FullItemNamed[Type];
            }
        }

        public override string ItemDescription
        {
            get
            {
                string description = m_material.Descriptions[Type];
                if (m_quality.Description.Length > 0)
                    description += "\n\n\n" + m_quality.Description;
                if (m_material.Attributes[Type].ContainsKey("RobesPreventBoots"))
                    description += "\n\n\n" + "The length of this item prevents boots from being worn.";
                return description;
            }
        }

        public override string FlavorDescription
        {
            get 
            {
                return "";
            }
        }

        public ArmorWeight Weight
        {
            get 
            {
                ArmorWeight weight = ArmorWeight.Light;
                
                if (m_material.Attributes[Type].ContainsKey("StandardArmor"))
                    weight = ArmorWeight.Standard;
                else if (m_material.Attributes[Type].ContainsKey("HeavyArmor"))
                    weight = ArmorWeight.Heavy;

                if (m_quality.Attributes.ContainsKey("ArmorHeavier"))
                {
                    switch (weight)
                    {
                        case ArmorWeight.Heavy:
                        case ArmorWeight.Standard:
                            weight = ArmorWeight.Heavy;
                            break;
                        case ArmorWeight.Light:
                            weight = ArmorWeight.Standard;
                            break;
                    }
                }

                if (m_quality.Attributes.ContainsKey("ArmorLighter"))
                {
                    switch (weight)
                    {
                        case ArmorWeight.Heavy:
                            weight = ArmorWeight.Standard;
                            break;
                        case ArmorWeight.Standard:
                            weight = ArmorWeight.Light;
                            break;
                    }
                }

                return weight;
            }
        }

        public int StaminaBonus
        {
            get 
            {
                return m_staminaBonus;
            }
        }

        public double Evade
        {
            get 
            {
                return m_evade;
            }
        }

        public string Type
        {
            get
            {
                return Attributes["Type"];
            }
        }

        public override bool ContainsAttribute(string key)
        {
            if (m_material.Attributes[Type].ContainsKey(key))
                return true;

            if (m_quality.Attributes.ContainsKey(key))
                return true;

            return base.ContainsAttribute(key);
        }

        public override string GetAttribute(string key)
        {
            if (m_material.Attributes[Type].ContainsKey(key))
                return m_material.Attributes[Type][key];

            if (m_quality.Attributes.ContainsKey(key))
                return m_quality.Attributes[key];

            return base.GetAttribute(key);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3}", Type, m_material.FullItemNamed[Type], m_material.MaterialName, m_quality.Name);
        }

        private void Calculate()
        {
            BaseArmorStats.Instance.LoadMappingIntoAttributes(this);

            int baseArmorStm = int.Parse(Attributes["BaseStaminaBonus"], CultureInfo.InvariantCulture);

            double staminaModifier = 1;

            double materialBonus = int.Parse(m_material.MaterialAttributes["StaminaBonus"], CultureInfo.InvariantCulture) / 100.0;
            staminaModifier += materialBonus;

            double qualityBonus = 0;
            string qualityString = m_quality.Attributes.ContainsKey("ArmorStaminaModifier") ? m_quality.Attributes["ArmorStaminaModifier"] : null;
            if (qualityString != null)
                qualityBonus = int.Parse(qualityString, CultureInfo.InvariantCulture) / 100.0;
            staminaModifier += qualityBonus;

            m_staminaBonus = (int)Math.Round(baseArmorStm * staminaModifier);

            // No matter how bad it is, it is better than nothing...
            m_staminaBonus = Math.Max(m_staminaBonus, 1);

            m_evade = m_material.MaterialAttributes.ContainsKey("EvadeBonus") ? 
                int.Parse(m_material.Attributes[Type]["EvadeBonus"], CultureInfo.InvariantCulture) : 0;
        }

        public override void ReadXml(XmlReader reader)
        {
            m_material = ItemFactory.Instance.MaterialFactory.GetMaterial(Type, reader.ReadElementContentAsString());
            m_quality = ItemFactory.Instance.QualityFactory.GetQuality(reader.ReadElementContentAsString());
            Calculate();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", Type);
            writer.WriteElementString("Material", m_material.MaterialName);
            writer.WriteElementString("Quality", m_quality.Name);
        }
    }
}
