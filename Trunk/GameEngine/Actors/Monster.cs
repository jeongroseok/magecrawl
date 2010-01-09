using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Affects;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal enum MonsterAction
    {
        DidAction,
        DidMove
    }

    internal sealed class Monster : Character, ICloneable
    {
        // Share one RNG between monsters
        private static TCODRandom m_random;
        private double CTAttackCost;

        static Monster()
        {
            m_random = new TCODRandom();
        }

        public Monster(string name, Point p, int maxHP, int vision, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, maxHP, vision, ctIncreaseModifer, ctMoveCost, ctActCost)
        {
            CTAttackCost = ctAttackCost;
        }

        public object Clone()
        {
            Monster newMonster = (Monster)this.MemberwiseClone();

            if (m_affects.Count > 0)
                throw new NotImplementedException("Have not implemented Clone() on monster when Affects are on it");

            newMonster.m_affects = new List<AffectBase>();

            return newMonster;
        }

        internal MonsterAction Action(CoreGameEngine engine)
        {
            if (engine.FOVManager.VisibleSingleShot(engine.Map, Position, Vision, engine.Player.Position))
            {
                // TODO - This should respect FOV
                IList<Point> pathToPlayer = engine.PathToPoint(this, engine.Player.Position, false, false, true);

                // If we are next to the player
                if (pathToPlayer != null && pathToPlayer.Count == 1)
                {
                    if (engine.Attack(this, engine.Player.Position))
                        return MonsterAction.DidAction;
                }

                // We have a way to get to player
                if (pathToPlayer != null && pathToPlayer.Count > 0)
                {
                    Direction towardsPlayer = PointDirectionUtils.ConvertTwoPointsToDirection(Position, pathToPlayer[0]);
                    if (engine.Move(this, towardsPlayer))
                        return MonsterAction.DidMove;
                }
            }

            // We have nothing else to do, so wander
            return WanderRandomly(engine);
        }

        private MonsterAction WanderRandomly(CoreGameEngine engine)
        {
            foreach(Direction d in DirectionUtils.GenerateDirectionList())
            {
                if (engine.Move(this, d))
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
                return new DiceRoll(1, 1);
            }
        }

        public override double MeleeSpeed
        {
            get
            {
                return CTAttackCost;
            }
        }

        public override IWeapon CurrentWeapon
        {
            get
            {
                return new GameEngine.Weapons.MeleeWeapon(this);
            }
        }

        #region SaveLoad

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            CTAttackCost = Double.Parse(reader.ReadElementContentAsString());
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", "Monster");
            base.WriteXml(writer);
            writer.WriteElementString("MeleeSpeed", CTAttackCost.ToString());
        }

        #endregion
    }
}
