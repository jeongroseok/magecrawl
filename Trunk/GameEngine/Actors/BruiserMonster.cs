using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class BruiserMonster : Monster
    {
        public BruiserMonster(string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
        }

        public override void Action(CoreGameEngine engine)
        {
            if (IsPlayerVisible(engine))
            {
                UpdateKnownPlayerLocation(engine);
                List<Point> pathToPlayer = GetPathToPlayer(engine);
                if (pathToPlayer != null && pathToPlayer.Count == 1)
                {
                    if (engine.UseMonsterSkill(this, SkillType.DoubleSwing, engine.Player.Position))
                        return;
                }
            }

            DefaultAction(engine);
        }
    }
}
