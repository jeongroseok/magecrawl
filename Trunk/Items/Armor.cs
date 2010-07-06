using System;
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
                    description += "\n\n\n";
                return description + m_quality.Description;
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
                else
                    weight = ArmorWeight.Light;

                if (m_quality.Attributes.ContainsKey("ArmorHeavy"))
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
                        case ArmorWeight.None:
                            weight = ArmorWeight.Light;
                            break;
                    }
                }

                if (m_quality.Attributes.ContainsKey("ArmorLight"))
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

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3}", Type, m_material.FullItemNamed[Type], m_material.MaterialName, m_quality.Name);
        }

        private void Calculate()
        {
            int baseArmorStm = 0;
            if (m_material.Attributes[Type].ContainsKey("StaminaBonus"))
                baseArmorStm = int.Parse(m_material.Attributes[Type]["StaminaBonus"]);

            double materialBonusFactor = 1.0;
            if (m_quality.Attributes.ContainsKey("ArmorStaminaModifier"))
                materialBonusFactor += double.Parse(m_quality.Attributes["ArmorStaminaModifier"]);

            int stmWithBonusFactor = (int)Math.Round(baseArmorStm * materialBonusFactor);

            // Higher quality should always give at least 1 point of stamina.
            if (materialBonusFactor > 1.0 && stmWithBonusFactor == baseArmorStm)
                m_staminaBonus = stmWithBonusFactor + 1;
            else
                m_staminaBonus = stmWithBonusFactor;

            if (m_material.Attributes[Type].ContainsKey("EvadeBonus"))
                m_evade = int.Parse(m_material.Attributes[Type]["EvadeBonus"]);
            else
                m_evade = 0;
        }

        public override void ReadXml(XmlReader reader)
        {
            m_material = ItemFactory.Instance.MaterialFactory.GetMaterial(Type, reader.ReadElementContentAsString());
            m_quality = ItemFactory.Instance.CraftsmanFactory.GetQuality(reader.ReadElementContentAsString());
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
