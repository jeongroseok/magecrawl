using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Magecrawl.Interfaces;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.Items.WeaponRanges;

namespace Magecrawl.Items
{
    public abstract class Weapon : Item, IWeapon, IWeaponVerb, IXmlSerializable
    {
        protected IWeaponRange m_weaponRange;

        internal Weapon() : base()
        {
        }

        internal Weapon(IWeaponRange weaponRange) : base()
        {
            m_weaponRange = weaponRange;
            Attributes["Type"] = m_weaponRange.Name;
        }

        public abstract DiceRoll Damage { get; }
        public abstract bool IsRanged { get; }
        public abstract bool IsLoaded { get; }
        public abstract double CTCostToAttack { get; }

        public string AttackVerb
        {
            get
            {
                return ((IWeaponVerb)m_weaponRange).AttackVerb;
            }
        }

        public string Type
        {
            get
            {
                return m_weaponRange.Name;
            }
        }

        public virtual void LoadWeapon()
        {
            throw new System.InvalidOperationException("LoadWeapon on non StatsBasedRangedWeapon?");
        }

        public virtual void UnloadWeapon()
        {
            throw new System.InvalidOperationException("UnloadWeapon on non StatsBasedRangedWeapon?");
        }

        // Needs to be single point of access in GameEngine so we can filter
        public List<EffectivePoint> CalculateTargetablePoints(Point wielderPosition)
        {
            return m_weaponRange.CalculateTargetablePoints(this, wielderPosition);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            m_weaponRange = WeaponRangeFactory.Create(reader.ReadElementContentAsString());
            Attributes["Type"] = m_weaponRange.Name;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", Type);
            writer.WriteElementString("TargettingType", Type);
        }
    }
}
