using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.EngineInterfaces
{
    public interface IGameEngineCore
    {
        void SendTextOutput(string s);
        void DamageTarget(ICharacterCore attacker, int damage, ICharacterCore target);
        void MonsterNoticeRangedAttack(ICharacterCore monster, Point positionAttackCameFrom);
        ICharacterCore FindTargetAtPosition(Point attackTarget);

        ICharacterCore Player
        {
            get;
        }
    }
}
