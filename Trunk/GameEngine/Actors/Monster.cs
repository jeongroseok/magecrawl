using System;
using System.Collections.Generic;
using libtcod;
using Magecrawl.GameEngine.Actors.MonsterAI;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class Monster : Character, IMonster
    {
        // Share one RNG between monsters
        protected static TCODRandom m_random = new TCODRandom();

        private List<IMonsterTactic> m_tactics;

        public SerializableDictionary<string, string> Attributes { get; set; }

        protected double CTAttackCost { get; set; }
        private DiceRoll m_damage;
        private int m_level;
        private string m_baseType;
        public bool Intelligent { get; private set; }

        public Point PlayerLastKnownPosition { get; set; }

        public Monster(string baseType, string name, int level, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade,
                       double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost, List<IMonsterTactic> tactics)
            : base(name, p, vision, ctIncreaseModifer, ctMoveCost, ctActCost)
        {
            m_baseType = baseType;
            m_level = level;
            CTAttackCost = ctAttackCost;
            m_currentHP = maxHP;
            m_maxHP = maxHP;
            m_damage = damage;
            PlayerLastKnownPosition = Point.Invalid;
            m_evade = evade;
            Intelligent = intelligent;
            Attributes = new SerializableDictionary<string, string>();
            m_tactics = tactics;
            m_tactics.ForEach(t => t.SetupAttributesNeeded(this));
        }
        
        public string BaseType
        {
            get 
            {
                return m_baseType;
            }
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

        // Monsters don't have stamina, so treat as damage
        public override void DamageJustStamina(int dmg)
        {
            m_currentHP -= dmg;
        }

        public override bool IsDead
        {
            get
            {
                return CurrentHP <= 0;
            }
        }

        public void Action(CoreGameEngine engine)
        {
            bool playerIsVisible = IsPlayerVisible(engine);
            if (playerIsVisible)
                UpdateKnownPlayerLocation(engine);

            m_tactics.ForEach(t => t.NewTurn(this));

            foreach (IMonsterTactic tactic in m_tactics)
            {
                if (playerIsVisible || !tactic.NeedsPlayerLOS)
                {
                    if (tactic.CouldUseTactic(engine, this))
                    {
                        if (tactic.UseTactic(engine, this))
                            return;
                    }
                }
            }
            throw new InvalidOperationException("Made it to the end of Action of Monster");
        }

        public void NoticeRangedAttack(Point attackerPosition)
        {
            PlayerLastKnownPosition = attackerPosition;
        }

        protected void UpdateKnownPlayerLocation(CoreGameEngine engine)
        {
            PlayerLastKnownPosition = engine.Player.Position;
        }

        protected bool IsPlayerVisible(CoreGameEngine engine)
        {
            return engine.FOVManager.VisibleSingleShot(engine.Map, Position, Vision, engine.Player.Position);
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
            writer.WriteElementString("Type", m_baseType);
            writer.WriteElementString("Level", m_level.ToString());
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
