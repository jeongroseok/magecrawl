using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Actors.MonsterAI;

namespace Magecrawl.GameEngine.Actors
{
    internal class RangedMonster : Monster
    {
        private List<IMonsterTactic> m_tactics;

        public RangedMonster(string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
            m_tactics = new List<IMonsterTactic>() { new KeepAwayFromMeleeRangeIfAbleTactic(), new UseSlingStoneTactic(), 
                new KeepAwayFromPlayerIfAbleTactic(), new PossiblyRunFromPlayerTactic(), new WaitTactic() };
            m_tactics.ForEach(t => t.SetupAttributesNeeded(this));
        }

        public override void Action(CoreGameEngine engine)
        {
            DoActionList(engine, m_tactics);
        }
    }
}