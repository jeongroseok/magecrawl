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
    internal abstract class WeaponBase : Item, IWeapon
    {
        protected DiceRoll m_damage;
        protected double m_ctCostToAttack;

        protected WeaponBase(string name, string itemDescription, string flavorText) : base(name, itemDescription, flavorText)
        {
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        internal ICharacter Owner { get; set; }

        public abstract string AttackVerb
        {
            get;
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

        public virtual bool IsRanged
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsLoaded
        {
            get
            {
                return true;
            }
            internal set
            {
                throw new System.InvalidOperationException("Can't set loaded on WeaponBase");
            }
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                List<ItemOptions> optionList = new List<ItemOptions>();
                
                if (CoreGameEngine.Instance.Player.CurrentWeapon == this)
                    optionList.Add(new ItemOptions("Unequip", true));
                else if (CoreGameEngine.Instance.Player.SecondaryWeapon == this)
                    optionList.Add(new ItemOptions("Unequip as Secondary", true));
                else
                {
                    optionList.Add(new ItemOptions("Equip", true));
                    optionList.Add(new ItemOptions("Equip as Secondary", true));
                    optionList.Add(new ItemOptions("Drop", true));
                }
                
                return optionList;
            }
        }

        public abstract List<EffectivePoint> CalculateTargetablePoints();

        public float EffectiveStrengthAtPoint(Point pointOfInterest)
        {
            return CalculateTargetablePoints().Where(p => p.Position == pointOfInterest).SingleOrDefault().EffectiveStrength; // If for some reason we ask for a time we can't target, Default will give us 0
        }

        public bool PositionInTargetablePoints(Point pointOfInterest)
        {
            return EffectivePoint.PositionInTargetablePoints(pointOfInterest, CalculateTargetablePoints());
        }
    }
}
