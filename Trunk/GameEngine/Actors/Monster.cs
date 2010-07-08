using System;
using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.Effects;
using Magecrawl.Interfaces;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal abstract class Monster : Character, ICloneable
    {
        // Share one RNG between monsters
        protected static TCODRandom m_random = new TCODRandom();

        public SerializableDictionary<string, string> Attributes { get; set; }

        protected double CTAttackCost { get; set; }
        private DiceRoll m_damage;
        public bool Intelligent { get; private set; }

        public Point PlayerLastKnownPosition { get; set; }

        public Monster(string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade,
                       double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, vision, ctIncreaseModifer, ctMoveCost, ctActCost)
        {
            CTAttackCost = ctAttackCost;
            m_currentHP = maxHP;
            m_maxHP = maxHP;
            m_damage = damage;
            PlayerLastKnownPosition = Point.Invalid;
            m_evade = evade;
            Intelligent = intelligent;
            Attributes = new SerializableDictionary<string, string>();
        }

        public object Clone()
        {
            Monster newMonster = (Monster)this.MemberwiseClone();

            if (m_effects.Count > 0)
                throw new NotImplementedException("Have not implemented Clone() on monster when Effects are on it");

            newMonster.m_effects = new List<StatusEffect>();
            newMonster.Attributes = new SerializableDictionary<string, string>(Attributes);

            return newMonster;
        }

        public override IWeapon CurrentWeapon
        {
            get
            {
                return CoreGameEngine.Instance.ItemFactory.CreateMeleeWeapon(this);
            }
        }
        
        private int m_currentHP;
        public override int CurrentHP 
        {
            get
            {
                return m_currentHP;
            }
        }

        private int m_maxHP;
        public override int MaxHP 
        {
            get
            {
                return m_maxHP;
            }
        }


        // Returns amount actually healed by
        public override int Heal(int toHeal, bool magical)
        {
            int previousHealth = CurrentHP;
            m_currentHP = Math.Min(CurrentHP + toHeal, MaxHP);
            return CurrentHP - previousHealth;
        }

        public override void Damage(int dmg)
        {
            m_currentHP -= dmg;
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
            }
            WanderRandomly(engine);   // We have nothing else to do, so wander                
            return;
        }

        public void NoticeRangedAttack(Point attackerPosition)
        {
            PlayerLastKnownPosition = attackerPosition;
        }

        #region ActionParts

        protected void UpdateKnownPlayerLocation(CoreGameEngine engine)
        {
            PlayerLastKnownPosition = engine.Player.Position;
        }

        protected bool WalkTowardsLastKnownPosition(CoreGameEngine engine)
        {
            if (PlayerLastKnownPosition == Point.Invalid)
                return false;

            List<Point> pathTowards = engine.PathToPoint(this, PlayerLastKnownPosition, Intelligent, false, true);
            if (pathTowards == null || pathTowards.Count == 0)
            {
                PlayerLastKnownPosition = Point.Invalid;
                return false;
            }
            else
            {
                return MoveOnPath(engine, pathTowards);
            }
        }

        protected List<Point> GetPathToCharacter(CoreGameEngine engine, ICharacter c)
        {
            return engine.PathToPoint(this, c.Position, Intelligent, false, true);
        }

        protected List<Point> GetPathToPlayer(CoreGameEngine engine)
        {
            return engine.PathToPoint(this, engine.Player.Position, Intelligent, false, true);
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

        private bool MoveCore(CoreGameEngine engine, Direction direction)
        {
            if (engine.Move(this, direction))
                return true;
            Point position = PointDirectionUtils.ConvertDirectionToDestinationPoint(Position, direction);
            if (Intelligent && engine.Operate(this, position))
                return true;
            return false;
        }

        protected bool MoveOnPath(CoreGameEngine engine, List<Point> path)
        {
            Direction nextPosition = PointDirectionUtils.ConvertTwoPointsToDirection(Position, path[0]);
            return MoveCore(engine, nextPosition);
        }

        protected bool MoveAwayFromPlayer(CoreGameEngine engine)
        {
            Direction directionTowardsPlayer = PointDirectionUtils.ConvertTwoPointsToDirection(Position, engine.Player.Position);
            if (MoveCore(engine, PointDirectionUtils.GetDirectionOpposite(directionTowardsPlayer)))
                return true;

            foreach (Direction attemptDirection in PointDirectionUtils.GetDirectionsOpposite(directionTowardsPlayer))
            {
                if (MoveCore(engine, attemptDirection))
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
            foreach (Direction d in DirectionUtils.GenerateRandomDirectionList())
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

        public override double MeleeCTCost
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

            m_currentHP = reader.ReadElementContentAsInt();
            m_maxHP = reader.ReadElementContentAsInt();

            PlayerLastKnownPosition.ReadXml(reader);

            // HACK HACK - When collapsed monster classes into IMonsterTactics, we shouldn't need this
            Attributes.Clear();
            Attributes.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", Name);
            base.WriteXml(writer);
            writer.WriteElementString("MeleeSpeed", CTAttackCost.ToString());
            m_damage.WriteXml(writer);

            writer.WriteElementString("CurrentHP", CurrentHP.ToString());
            writer.WriteElementString("MaxHP", MaxHP.ToString());

            PlayerLastKnownPosition.WriteToXml(writer, "PlayerLastKnownPosition");

            Attributes.WriteXml(writer);
        }

        #endregion
    }
}
