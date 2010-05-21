using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class SprinterMonster : Monster
    {
        public SprinterMonster(string name, Point p, int maxHP, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
        }

        public override void Action(CoreGameEngine engine)
        {
            if (IsPlayerVisible(engine))
            {
                UpdateKnownPlayerLocation(engine);
                List<Point> pathToPlayer = GetPathToPlayer(engine);
                if (pathToPlayer != null && pathToPlayer.Count == 2)
                {
                    if (engine.UseSkill(this, SkillType.Rush, engine.Player.Position))
                        return;
                }
            }

            DefaultAction(engine);
        }
    }
}
