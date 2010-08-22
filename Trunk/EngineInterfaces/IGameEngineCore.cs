using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.EngineInterfaces
{
    public interface IGameEngineCore
    {
        void SendTextOutput(string s);
        
        void DamageTarget(ICharacterCore attacker, int damage, ICharacterCore target);
        bool Wait(ICharacterCore c);
        bool Move(ICharacterCore c, Direction direction);
        bool Attack(ICharacterCore attacker, Point attackTarget);
        bool Operate(ICharacterCore characterOperating, Point pointToOperateAt);

        int GetMonsterVisionBonus();
        void MonsterNoticeRangedAttack(ICharacterCore monster, Point positionAttackCameFrom);
        
        ICharacterCore FindTargetAtPosition(Point attackTarget);
        List<Point> PathToPoint(ICharacterCore actor, Point dest, bool canOperate, bool usePlayerLOS, bool monstersBlockPath);
        List<ICharacterCore> MonstersInCharactersLOS(ICharacterCore character);

        ICharacterCore Player
        {
            get;
        }

        IMapCore Map
        {
            get;
        }

        IFOVManager FOVManager
        {
            get;
        }

        IMonsterSkillEngine MonsterSkillEngine
        {
            get;
        }
    }

    public static class CoreGameEngineInstance
    {
        public static IGameEngineCore Instance { get; set; }
    }
}
