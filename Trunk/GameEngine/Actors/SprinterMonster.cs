using System;
using System.Collections.Generic;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Actors.MonsterAI;

namespace Magecrawl.GameEngine.Actors
{
    internal class SprinterMonster : Monster
    {
        List<IMonsterTactic> m_tactics;

        public SprinterMonster(string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
            m_tactics = new List<IMonsterTactic>() { new RushTactic() };
            m_tactics.ForEach(t => t.SetupAttributesNeeded(this));
        }

        public override void Action(CoreGameEngine engine)
        {
            DoActionList(engine, m_tactics);
        }
    }
}
