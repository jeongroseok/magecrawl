﻿using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.GameEngine.Items;

namespace Magecrawl.GameEngine.Actors
{
    internal class RangedMonster : Monster
    {
        private bool m_seenPlayerBefore;

        public RangedMonster(string name, Point p, int maxHP, int vision, DiceRoll damage, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, vision, damage, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
            m_seenPlayerBefore = false;
        }

        private bool IfNearbyEnemeiesTryToMoveAway(CoreGameEngine engine)
        {
            if (OtherNearbyEnemies(engine))
                return MoveAwayFromPlayer(engine);
            return false;
        }

        public override void Action(CoreGameEngine engine)
        {
            if (IsPlayerVisible(engine))
            {
                List<Point> pathToPlayer = GetPathToPlayer(engine);
                int distanceToPlayer = pathToPlayer.Count;
                if (distanceToPlayer == 1)
                {
                    bool moveSucessful = IfNearbyEnemeiesTryToMoveAway(engine);
                    if (!moveSucessful)
                        Attack(engine, false, null);

                    return;
                }
                else
                {
                    if (!CurrentWeapon.IsRanged)
                    {
                        engine.SwapPrimarySecondaryWeapons(this, true);
                    }
                    else if (!CurrentWeapon.IsLoaded)
                    {
                        engine.ReloadWeapon(this);
                    }
                    else if (CurrentWeapon.EffectiveStrengthAtPoint(engine.Player.Position) > 0)
                    {
                        Attack(engine, true, pathToPlayer);
                    }
                    else
                    {
                        bool moveSucessful = IfNearbyEnemeiesTryToMoveAway(engine);
                        if (!moveSucessful)
                            engine.Move(this, PointDirectionUtils.ConvertTwoPointsToDirection(Position, pathToPlayer[0]));
                    }
                    return;
                }
            }
            else
            {
                if (!CurrentWeapon.IsLoaded)
                    engine.ReloadWeapon(this);
                else
                    WanderRandomly(engine);
                return;
            }
            throw new InvalidOperationException("RangedMonster Action should never reach end of statement");
        }

        private void Attack(CoreGameEngine engine, bool fromRange, List<Point> pathToPlayer)
        {
            if (fromRange)
            {
                if (!CurrentWeapon.IsRanged)
                    engine.SwapPrimarySecondaryWeapons(this, true);

                CoreGameEngine.Instance.RangedAttackOnPlayer(pathToPlayer);
            }
            else
            {
                if (CurrentWeapon.IsRanged)
                    engine.SwapPrimarySecondaryWeapons(this, true);
            }
            engine.Attack(this, engine.Player.Position);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_seenPlayerBefore = Boolean.Parse(reader.ReadElementContentAsString());         
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("SeenPlayerBefore", m_seenPlayerBefore.ToString());
        }
    }
}

