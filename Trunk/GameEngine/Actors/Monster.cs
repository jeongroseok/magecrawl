using System;
using System.Collections.Generic;
using System.Linq;
using libtcodWrapper;
using Magecrawl.GameEngine.Affects;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal abstract class Monster : Character, ICloneable
    {
        // Share one RNG between monsters
        protected static TCODRandom m_random;
        protected double CTAttackCost { get; set; }
        private DiceRoll m_damage;
        protected Point m_playerLastKnownPosition;

        public Monster(string name, Point p, int maxHP, int vision, DiceRoll damage, double defense, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, maxHP, vision, ctIncreaseModifer, ctMoveCost, ctActCost)
        {
            CTAttackCost = ctAttackCost;
            m_damage = damage;
            m_playerLastKnownPosition = Point.Invalid;
            m_defense = defense;
            m_evade = evade;
        }

        static Monster()
        {
            m_random = new TCODRandom();
        }

        public object Clone()
        {
            Monster newMonster = (Monster)this.MemberwiseClone();

            if (CurrentWeapon.GetType() != typeof(MeleeWeapon))
                newMonster.Equip(CoreGameEngine.Instance.ItemFactory.CreateItem(CurrentWeapon.DisplayName));

            if (SecondaryWeapon.GetType() != typeof(MeleeWeapon))
                newMonster.EquipSecondaryWeapon((IWeapon)CoreGameEngine.Instance.ItemFactory.CreateItem(SecondaryWeapon.DisplayName));

            if (m_affects.Count > 0)
                throw new NotImplementedException("Have not implemented Clone() on monster when Affects are on it");

            newMonster.m_affects = new List<AffectBase>();

            return newMonster;
        }

        public abstract void Action(CoreGameEngine engine);

        protected void DefaultAction(CoreGameEngine engine)
        {
            if (IsPlayerVisible(engine))
            {
                UpdateKnownPlayerLocation(engine);
                List<Point> pathToPlayer = GetPathToPlayer(engine);

                if (IsNextToPlayer(engine, pathToPlayer))
                {
                    if (AttackPlayer(engine))
                        return;
                }

                if (HasPathToPlayer(engine, pathToPlayer))
                {
                    if (MoveOnPath(engine, pathToPlayer))
                        return;
                }
            }
            else
            {
                if (WalkTowardsLastKnownPosition(engine))
                    return;
                
                WanderRandomly(engine);   // We have nothing else to do, so wander                
                return;
            }
        }

        #region ActionParts

        protected void UpdateKnownPlayerLocation(CoreGameEngine engine)
        {
            m_playerLastKnownPosition = engine.Player.Position;
        }

        protected bool WalkTowardsLastKnownPosition(CoreGameEngine engine)
        {
            if (m_playerLastKnownPosition == Point.Invalid)
                return false;

            List<Point> pathTowards = engine.PathToPoint(this, m_playerLastKnownPosition, false, false, true);
            if (pathTowards == null || pathTowards.Count == 0)
            {
                m_playerLastKnownPosition = Point.Invalid;
                return false;
            }
            else
            {
                return MoveOnPath(engine, pathTowards);
            }
        }

        protected List<Point> GetPathToCharacter(CoreGameEngine engine, ICharacter c)
        {
            return engine.PathToPoint(this, c.Position, false, false, true);
        }

        protected List<Point> GetPathToPlayer(CoreGameEngine engine)
        {
            return engine.PathToPoint(this, engine.Player.Position, false, false, true);
        }

        protected bool IsNextToPlayer(CoreGameEngine engine, List<Point> pathToPlayer)
        {
            return pathToPlayer != null && pathToPlayer.Count == 1;
        }

        protected bool HasPathToPlayer(CoreGameEngine engine, List<Point> pathToPlayer)
        {
            return pathToPlayer != null && pathToPlayer.Count > 0;
        }

        protected bool AttackPlayer(CoreGameEngine engine)
        {
            if (engine.Attack(this, engine.Player.Position))
                return true;
            return false;
        }

        protected bool MoveTowardsPlayer(CoreGameEngine engine)
        {
            return MoveOnPath(engine, GetPathToPlayer(engine));
        }

        protected bool MoveOnPath(CoreGameEngine engine, List<Point> path)
        {
            Direction nextPosition = PointDirectionUtils.ConvertTwoPointsToDirection(Position, path[0]);
            if (engine.Move(this, nextPosition))
                return true;
            return false;
        }

        protected bool MoveAwayFromPlayer(CoreGameEngine engine)
        {
            Direction directionTowardsPlayer = PointDirectionUtils.ConvertTwoPointsToDirection(Position, engine.Player.Position);
            if (engine.Move(this, PointDirectionUtils.GetDirectionOpposite(directionTowardsPlayer)))
                return true;
            
            foreach (Direction attemptDirection in PointDirectionUtils.GetDirectionsOpposite(directionTowardsPlayer))
            {
                if (engine.Move(this, attemptDirection))
                    return true;
            }
            return false;
        }

        protected List<ICharacter> OtherNearbyEnemies(CoreGameEngine engine)
        {
            return engine.MonstersInPlayerLOS().Where(x => PointDirectionUtils.NormalDistance(x.Position, engine.Player.Position) < 5).ToList();
        }

        protected bool AreOtherNearbyEnemies(CoreGameEngine engine)
        {
            return OtherNearbyEnemies(engine).Count() > 1;
        }

        protected bool IsPlayerVisible(CoreGameEngine engine)
        {
            return engine.FOVManager.VisibleSingleShot(engine.Map, Position, Vision, engine.Player.Position);
        }

        protected bool WanderRandomly(CoreGameEngine engine)
        {
            foreach (Direction d in DirectionUtils.GenerateDirectionList())
            {
                if (engine.Move(this, d))
                {
                    return true;
                }
            }

            // If nothing else, 'wait'
            engine.Wait(this);
            return false;
        }

        #endregion

        private double m_defense;
        public override double Defense
        {
            get
            {
                return m_defense;
            }
        }

        private double m_evade;
        public override double Evade
        {
            get
            {
                return m_evade;
            }
        }

        public override DiceRoll MeleeDamage
        {
            get
            {
                return m_damage;
            }
        }

        public override double MeleeSpeed
        {
            get
            {
                return CTAttackCost;
            }
        }

        #region SaveLoad

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            CTAttackCost = reader.ReadElementContentAsDouble();
            m_damage.ReadXml(reader);
            m_playerLastKnownPosition.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", Name);
            base.WriteXml(writer);
            writer.WriteElementString("MeleeSpeed", CTAttackCost.ToString());
            m_damage.WriteXml(writer);
            m_playerLastKnownPosition.WriteToXml(writer, "PlayerLastKnownPosition");
        }

        #endregion
    }
}
