using System;
using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class MeleeWeapon : WeaponBase
    {
        internal MeleeWeapon(ICharacter owner) : base(owner, "Melee", "Your Natural Weapons", "")
        {
        }

        public override DiceRoll Damage
        {
            get 
            {
                return Owner.MeleeDamage;
            }
        }

        public override double CTCostToAttack
        {
            get
            {
                return Owner.MeleeSpeed;
            }
        }

        public override List<ItemOptions> PlayerOptions
        {
            get
            {
                return new List<ItemOptions>() 
                {
                    new ItemOptions("Equip", true),
                };
            }
        }

        public override List<EffectivePoint> CalculateTargetablePoints()
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(-1, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(0, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(0, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(1, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(-1, -1), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(-1, 1), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(1, -1), 1.0f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, Owner.Position, Owner.Vision, true);

            return targetablePoints;
        }

        public override string AttackVerb
        {
            get
            {
                return "strikes at";
            }
        }
    }
}
