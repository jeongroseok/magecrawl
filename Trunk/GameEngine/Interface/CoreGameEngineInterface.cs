using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.EngineInterfaces;
using Magecrawl.GameEngine.Actors;

namespace Magecrawl.GameEngine.Interface
{
    internal class CoreGameEngineInterface : IGameEngineCore
    {
        public static CoreGameEngineInterface Instance = new CoreGameEngineInterface();

        private CoreGameEngine m_engine;
        private CoreGameEngineInterface()
        {
            m_engine = CoreGameEngine.Instance;
        }

        public void SendTextOutput(string s)
        {
            m_engine.SendTextOutput(s);
        }

        public void DamageTarget(ICharacterCore attacker, int damage, ICharacterCore target)
        {
            m_engine.CombatEngine.DamageTarget((Character)attacker, damage, (Character)target);
        }

        public void MonsterNoticeRangedAttack(ICharacterCore monster, Utilities.Point positionAttackCameFrom)
        {
            ((Monster)monster).NoticeRangedAttack(positionAttackCameFrom);
        }

        public ICharacterCore FindTargetAtPosition(Utilities.Point attackTarget)
        {
            return m_engine.CombatEngine.FindTargetAtPosition(attackTarget);
        }

        public ICharacterCore Player
        {
            get 
            {
                return m_engine.Player;
            }
        }
    }
}
