using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal abstract class WeaponBase : IWeapon, Item
    {
        protected ICharacter m_owner;
        protected string m_name;
        protected DiceRoll m_damage;
        protected string m_itemDescription;
        protected string m_flavorText;
        protected double m_ctCostToAttack;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        internal ICharacter Owner
        {
            get
            {
                return m_owner;
            }
            set
            {
                m_owner = value;
            }
        }

        public virtual double CTCostToAttack
        {
            get
            {
                return m_ctCostToAttack;
            }
        }

        public virtual DiceRoll Damage
        {
            get
            {
                return m_damage;
            }
        }

        public virtual string DisplayName
        {
            get
            {
                return m_name;
            }
        }

        public string ItemDescription
        {
            get
            {
                return m_itemDescription;
            }
        }

        public string FlavorDescription
        {
            get
            {
                return m_flavorText;
            }
        }

        public virtual bool IsRanged
        {
            get
            {
                return false;
            }
        }

        #region SaveLoad

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", m_name);
        }

        #endregion

        public virtual List<ItemOptions> PlayerOptions
        {
            get
            {
                List<ItemOptions> optionList = new List<ItemOptions>();
                if (m_owner == null)
                    optionList.Add(new ItemOptions("Equip", true));
                else
                    optionList.Add(new ItemOptions("Unequip", true));
                optionList.Add(new ItemOptions("Drop", true));
                return optionList;
            }
        }

        public abstract List<EffectivePoint> CalculateTargetablePoints();

        public float EffectiveStrengthAtPoint(Point pointOfInterest)
        {
            return CalculateTargetablePoints().Where(p => p.Position == pointOfInterest).Single().EffectiveStrength;
        }

        public bool PositionInTargetablePoints(Point pointOfInterest)
        {
            return EffectivePoint.PositionInTargetablePoints(pointOfInterest, CalculateTargetablePoints());
        }
    }
}
