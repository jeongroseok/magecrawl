using System;
using System.Collections.Generic;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.GameEngine.Items;

namespace Magecrawl.GameEngine.Actors
{
    internal class RangedMonster : Monster
    {
        private bool m_isStoneLoaded;
        private WeaponBase m_equipedWeapon;
        private bool m_seenPlayerBefore;

        public RangedMonster(string name, Point p, int maxHP, int vision, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, vision, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
            m_isStoneLoaded = m_random.Chance(75);  // Sometimes it doesn't have a stone in the sling.
            m_equipedWeapon = null;
            m_seenPlayerBefore = false;
        }

        public override void Action(CoreGameEngine engine)
        {
            if (IsPlayerVisible(engine))
            {
                List<Point> pathToPlayer = GetPathToPlayer(engine);
                int distanceToPlayer = pathToPlayer.Count;
                if (distanceToPlayer == 1)
                {
                    engine.Attack(this, engine.Player.Position);
                    return;
                }
                else
                {
                    if (m_isStoneLoaded)
                    {
                        m_equipedWeapon = (WeaponBase)engine.ItemFactory.CreateItem("Simple Sling");
                        m_equipedWeapon.Owner = this;

                        if (m_equipedWeapon.EffectiveStrengthAtPoint(engine.Player.Position) > 0)
                        {
                            engine.SendTextOutput(string.Format("{0} slings a stone at {1}.", Name, engine.Player.Name));
                            engine.Attack(this, engine.Player.Position);
                        }
                        else
                        {   
                            engine.Move(this, PointDirectionUtils.ConvertTwoPointsToDirection(Position, pathToPlayer[0]));
                        }

                        m_equipedWeapon = null;
                        m_isStoneLoaded = false;
                        return;
                    }
                    else
                    {
                        // Load stone.
                        m_isStoneLoaded = true;
                        engine.Wait(this);
                        return;
                    }
                }
            }
            else
            {
                if (!m_isStoneLoaded && m_seenPlayerBefore)
                {
                    // Load stone.
                    m_isStoneLoaded = true;
                    engine.Wait(this);
                    return;
                }

                WanderRandomly(engine);
                return;
            }
            throw new InvalidOperationException("RangedMonster Action should never reach end of statement");
        }

        public override IWeapon CurrentWeapon
        {
            get
            {
                if (m_equipedWeapon != null)
                    return m_equipedWeapon;
                else
                    return base.CurrentWeapon;
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_isStoneLoaded = Boolean.Parse(reader.ReadElementContentAsString());
            m_seenPlayerBefore = Boolean.Parse(reader.ReadElementContentAsString());
            
            m_equipedWeapon = ReadWeaponFromSave(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("StoneLoaded", m_isStoneLoaded.ToString());
            writer.WriteElementString("SeenPlayerBefore", m_seenPlayerBefore.ToString());

            WriteWeaponToSave(writer, m_equipedWeapon);
        }
    }
}

