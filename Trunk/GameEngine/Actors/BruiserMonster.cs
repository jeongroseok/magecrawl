using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class BruiserMonster : Monster
    {
        public BruiserMonster(string name, Point p, int maxHP, int vision, DiceRoll damage, double defense, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, vision, damage, defense, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
        }

        public override void Action(CoreGameEngine engine)
        {
            if (IsPlayerVisible(engine) && GetPathToPlayer(engine).Count == 1)
            {
                UpdateKnownPlayerLocation(engine);
                if (engine.UseSkill(this, SkillType.DoubleSwing, engine.Player.Position))
                    return;
            }

            DefaultAction(engine);
        }
    }
}
