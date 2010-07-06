using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Magecrawl.Interfaces;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Items
{
    public class MeleeWeapon : Weapon, IWeapon, IXmlSerializable
    {
        private ICharacter m_wielder;

        internal MeleeWeapon() : base(new WeaponRanges.MeleeRange())
        {
        }

        internal MeleeWeapon(ICharacter wielder) : base(new WeaponRanges.MeleeRange())
        {
            SetWielder(wielder);
        }

        public void SetWielder(ICharacter wielder)
        {
            m_wielder = wielder;
        }

        public override DiceRoll Damage
        {            
            get 
            {
                return m_wielder.MeleeDamage;
            }
        }

        public override double CTCostToAttack
        {
            get
            {
                return m_wielder.MeleeCTCost;
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
                throw new System.InvalidOperationException("Ask IsLoaded on melee weapon?");
            }
        }

        public override string ItemDescription
        {
            get
            {
                return "";
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
                return "Melee";
            }
        }

        public override string ToString()
        {
            if (m_wielder != null)
                return string.Format("MeleeWeapon - {0}", m_wielder.Name);
            else
                return "MelleWeapon - No Owner";
        }
    }
}
