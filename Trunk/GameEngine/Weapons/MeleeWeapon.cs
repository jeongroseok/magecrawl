using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal class MeleeWeapon : IWeapon
    {
        private CoreGameEngine m_engine;

        internal MeleeWeapon(CoreGameEngine engine)
        {
            m_engine = engine;
        }

        public DiceRoll Damage
        {
            get 
            {
                return new DiceRoll(1,2);
            }
        }

        public string Name
        {
            get 
            {
                return "Melee";
            }
        }

        public List<Point> TargetablePoints(Point characterPosition)
        {
            List<Point> targetablePoints = new List<Point>();

            targetablePoints.Add(m_engine.Player.Position + new Point(1, 0));
            targetablePoints.Add(m_engine.Player.Position + new Point(-1, 0));
            targetablePoints.Add(m_engine.Player.Position + new Point(0, 1));
            targetablePoints.Add(m_engine.Player.Position + new Point(0, -1));

            return targetablePoints;
        }
    }
}
