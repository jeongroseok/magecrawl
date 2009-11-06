using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal enum MonsterAction
    {
        DidAction,
        DidMove
    }

    internal sealed class Monster : Character
    {
        // Share one RNG between monsters
        private static TCODRandom m_random;
        static Monster()
        {
            m_random = new TCODRandom();
        }

        public Monster(Point p) : base(p.X, p.Y, 4, 4, 4, 1, 1, "Scary Monster")
        {
        }

        public override IWeapon CurrentWeapon
        {
            get
            {
                return new GameEngine.Weapons.MeleeWeapon(this);
            }
        }

        internal MonsterAction Action(CoreGameEngine engine)
        {
            if (engine.FOVManager.VisibleSingleShot(m_position, m_visionRange, engine.Player.Position))
            {
                // TODO - This should respect FOV
                IList<Point> pathToPlayer = engine.PathToPoint(this, engine.Player.Position, false);

                // If we are next to the player
                if (pathToPlayer != null && pathToPlayer.Count == 1)
                {
                    if (engine.Attack(this, engine.Player.Position))
                        return MonsterAction.DidAction;
                }

                // We have a way to get to player
                if (pathToPlayer != null && pathToPlayer.Count > 0)
                {
                    Direction towardsPlayer = PointDirectionUtils.ConvertTwoPointsToDirection(m_position, pathToPlayer[0]);
                    if (engine.Move(this, towardsPlayer))
                        return MonsterAction.DidMove;
                }
            }

            // We have nothing else to do, so wander
            return WanderRandomly(engine);
        }

        private MonsterAction WanderRandomly(CoreGameEngine engine)
        {
            for (int i = 0; i < 10; ++i)
            {
                Direction directionToTry = (Direction)m_random.GetRandomInt(1, 8);
                if (engine.Move(this, directionToTry))
                {
                    return MonsterAction.DidMove;
                }
            }

            // If nothing else, 'wait'
            engine.Wait(this);
            return MonsterAction.DidAction;
        }

        public override DiceRoll MeleeDamage
        {
            get
            {
                return new DiceRoll(1, 2);
            }
        }

        #region SaveLoad

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", "Monster");
            base.WriteXml(writer);
        }

        #endregion
    }
}
