using System;
using System.Xml;
using Magecrawl.Items.Interfaces;
using Magecrawl.Items.Materials;
using Magecrawl.Utilities;

namespace Magecrawl.Items
{
    internal class StatsBasedWeapon : Weapon
    {
        private Material m_material;
        private Quality m_quality;

        // Calculated from material and quality. Should not be saved.
        private DiceRoll m_damage;
        private double m_CTCost;

        internal StatsBasedWeapon() : base()
        {
        }

        internal StatsBasedWeapon(IWeaponRange weaponRange, Material material, Quality quality) : base(weaponRange)
        {
            m_material = material;
            m_quality = quality;
            Calculate();
        }

        private void Calculate()
        {
            m_damage = new DiceRoll(m_material.Attributes[Type]["Damage"]);
            if (m_quality.Attributes.ContainsKey("DamageModifier"))
                m_damage.Multiplier += double.Parse(m_quality.Attributes["DamageModifier"]);
            
            m_CTCost = double.Parse(m_material.Attributes[Type]["CTCostToAttack"]);
            if (m_quality.Attributes.ContainsKey("SpeedModifier"))
                m_CTCost += double.Parse(m_quality.Attributes["SpeedModifier"]);
        }

        public override DiceRoll Damage
        {
            get
            {
                return m_damage;
            }
        }

        public override double CTCostToAttack
        {
            get
            {
                return m_CTCost;
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

        public override string DisplayName
        {
            get
            {
                return m_material.FullItemNamed[Type];
            }
        }

        public override bool IsRanged
        {
            get
            {
                return false;
            }
        }

        public override bool IsLoaded
        {
            get
            {
                throw new InvalidCastException("IsLoaded on melee weapon?");
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3}", Type, DisplayName, m_material.MaterialName, m_quality.Name);
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            m_material = ItemFactory.Instance.MaterialFactory.GetMaterial(Type, reader.ReadElementContentAsString());
            m_quality = ItemFactory.Instance.CraftsmanFactory.GetQuality(reader.ReadElementContentAsString());
            Calculate();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("Material", m_material.MaterialName);
            writer.WriteElementString("Quality", m_quality.Name);
        }
    }
}
