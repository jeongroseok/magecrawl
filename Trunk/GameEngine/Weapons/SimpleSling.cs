using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class SimpleSling : RangedWeaponBase
    {
        public SimpleSling(string name, DiceRoll damage, double ctCost, string description, string flavorText)
        {
            m_itemDescription = description;
            m_flavorText = flavorText;
            m_owner = null;
            m_name = name;
            m_damage = damage;
            m_ctCostToAttack = ctCost;
        }

        public override List<EffectivePoint> CalculateTargetablePoints()
        {
            const int SimpleSlingRange = 5;
            const int SimpleSlingMinRange = 1;
            const int SimpleSlingFalloffStart = 3;
            const float SimpleSlingFalloffAmount = 0;

            List<EffectivePoint> targetablePoints = GenerateRangedTargetablePoints(SimpleSlingRange, SimpleSlingMinRange, SimpleSlingFalloffStart, SimpleSlingFalloffAmount);

            CoreGameEngine.Instance.FilterNotTargetablePointsFromList(targetablePoints, m_owner.Position, m_owner.Vision, true);

            return targetablePoints;
        }
    }
}
