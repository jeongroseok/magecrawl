using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class RangedMonster : Monster
    {
        private bool m_seenPlayerBefore;
        private int m_slingCooldown;

        public RangedMonster(string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
            m_seenPlayerBefore = false;
            m_slingCooldown = 0;
        }

        private bool CanUseSling()
        {
            return m_slingCooldown == 0;
        }

        private void UsedSling()
        {
            const int SlingCooldown = 3;
            m_slingCooldown = SlingCooldown;
        }


        private void NewTurn()
        {
            if (m_slingCooldown > 0)
                m_slingCooldown--;
        }

        private bool IfNearbyEnemeiesTryToMoveAway(CoreGameEngine engine)
        {
            if (AreOtherNearbyEnemies(engine))
                return MoveAwayFromPlayer(engine);
            return false;
        }

        public override void Action(CoreGameEngine engine)
        {
            NewTurn();
            if (IsPlayerVisible(engine))
            {
                UpdateKnownPlayerLocation(engine);
                int distanceToPlayer = GetPathToPlayer(engine).Count;
                if (distanceToPlayer == 1)
                {
                    bool moveSucessful = IfNearbyEnemeiesTryToMoveAway(engine);
                    if (!moveSucessful)
                        engine.Attack(this, engine.Player.Position);
                    return;
                }
                else
                {
                    if (CanUseSling())
                    {
                        if (engine.UseMonsterSkill(this, SkillType.SlingStone, engine.Player.Position))
                        {
                            UsedSling();
                            return;
                        }
                    }
                    bool moveSucessful = IfNearbyEnemeiesTryToMoveAway(engine);
                    if (!moveSucessful)
                    {
                        if (m_random.Chance(50))
                            MoveTowardsPlayer(engine);
                        else
                            engine.Wait(this);
                    }
                    return;
                }
            }
            else
            {
                if (WalkTowardsLastKnownPosition(engine))
                    return;

                WanderRandomly(engine);
                return;
            }
            throw new InvalidOperationException("RangedMonster Action should never reach end of statement");
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_seenPlayerBefore = Boolean.Parse(reader.ReadElementContentAsString());
            m_slingCooldown = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("SeenPlayerBefore", m_seenPlayerBefore.ToString());
            writer.WriteElementString("SlingsCooldown", m_slingCooldown.ToString());
        }
    }
}