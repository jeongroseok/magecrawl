using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class Spear : WeaponBase
    {
        public Spear(string name, DiceRoll damage, double ctCost, string description, string flavorText) : base(name, description, flavorText)
        {
            Owner = null;
            m_damage = damage;
            m_ctCostToAttack = ctCost;
        }

        public override List<EffectivePoint> CalculateTargetablePoints()
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(1, 0), .5f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(2, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(-1, 0), .5f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(-2, 0), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(0, 1), .5f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(0, 2), 1.0f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(0, -1), .5f));
            targetablePoints.Add(new EffectivePoint(Owner.Position + new Point(0, -2), 1.0f));

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, Owner.Position, Owner.Vision, true);

            return targetablePoints;
        }

        public override string AttackVerb
        {
            get
            {
                return "stabs";
            }
        }
    }
}
