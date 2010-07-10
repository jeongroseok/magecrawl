using System;
using System.Xml;
using Magecrawl.Items.Interfaces;
using Magecrawl.Items.Materials;
using Magecrawl.Utilities;
using System.Globalization;

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
            BaseWeaponStats.Instance.LoadMappingIntoAttributes(this);

            m_damage = new DiceRoll(Attributes["BaseDamage"]);
            m_damage.Add(new DiceRoll(m_material.MaterialAttributes["DamageBonus"]));
            if (m_quality.Attributes.ContainsKey("DamageModifier"))
                m_damage.Multiplier += 1.0 + (int.Parse(m_quality.Attributes["DamageModifier"], CultureInfo.InvariantCulture) / 100.0);

            m_CTCost = double.Parse(Attributes["BaseSpeed"], CultureInfo.InvariantCulture);
            m_CTCost += double.Parse(m_material.MaterialAttributes["ExtraCTCost"], CultureInfo.InvariantCulture);
            if (m_quality.Attributes.ContainsKey("SpeedModifier"))
                m_CTCost += 1.0 + (int.Parse(m_quality.Attributes["SpeedModifier"], CultureInfo.InvariantCulture) / 100.0);
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
