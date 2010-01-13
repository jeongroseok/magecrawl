using System;
using System.Collections.Generic;
using System.Xml;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    abstract internal class RangedWeaponBase : WeaponBase, Item
    {
        public override bool IsRanged
        {
            get
            {
                return true;
            }
        }

        public override bool IsLoaded { get; internal set; }

        protected List<EffectivePoint> GenerateRangedTargetablePoints(int range, int minDistance, int falloffDistance, float falloffPerSquare)
        {
            List<EffectivePoint> targetablePoints = new List<EffectivePoint>();

            for (int i = -range; i <= range; ++i)
            {
                for (int j = -range; j <= range; ++j)
                {
                    int distance = System.Math.Abs(i) + Math.Abs(j);
                    if ((distance <= range) && (distance >= minDistance))
                    {
                        float weaponStrength = 1.0f - (Math.Max(distance - falloffDistance, 0) * falloffPerSquare);
                        targetablePoints.Add(new EffectivePoint(new Point(Owner.Position.X + i, Owner.Position.Y + j), weaponStrength));
                    }
                }
            }
            return targetablePoints;
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            IsLoaded = Boolean.Parse(reader.ReadElementContentAsString());
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteElementString("IsLoaded", IsLoaded.ToString());
        }
    }
}
