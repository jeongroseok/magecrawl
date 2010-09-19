using System;
using System.Xml;
using Magecrawl.Items.Interfaces;
using Magecrawl.Items.Materials;

namespace Magecrawl.Items
{
    // Public since SL won't bind to internal objects
    public class StatsBasedRangedWeapon : StatsBasedWeapon
    {
        internal StatsBasedRangedWeapon() : base()
        {            
        }

        internal StatsBasedRangedWeapon(IWeaponRange weaponRange, Material material, Quality quality) : base(weaponRange, material, quality)
        {
            if (!weaponRange.IsRanged)
                throw new InvalidOperationException("StatsBasedRangedWeapon with no ranged weaponRange?");
        }

        public override bool IsRanged
        {
            get
            {
                return m_weaponRange.IsRanged;
            }
        }

        private bool m_isLoaded;
        public override bool IsLoaded
        {
            get
            {
                return IsRanged ? m_isLoaded : false;
            }
        }

        public override void LoadWeapon()
        {
            m_isLoaded = true;
        }

        public override void UnloadWeapon()
        {
            m_isLoaded = false;
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            if (IsRanged)
                m_isLoaded = bool.Parse(reader.ReadElementContentAsString());
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (IsRanged)
                writer.WriteElementString("IsLoaded", m_isLoaded.ToString());
        }
    }
}
