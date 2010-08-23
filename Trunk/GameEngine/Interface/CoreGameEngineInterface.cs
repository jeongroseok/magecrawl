using System.Collections.Generic;
using System.Linq;
using Magecrawl.Actors;
using Magecrawl.EngineInterfaces;
using Magecrawl.Maps;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interface
{
    internal class CoreGameEngineInterface : IGameEngineCore
    {
        public static CoreGameEngineInterface Instance = null;

        private CoreGameEngine m_engine;
        private CoreGameEngineInterface(CoreGameEngine engine)
        {
            m_engine = engine;
            CoreGameEngineInstance.Instance = this;
        }

        internal static void SetupCoreGameEngineInterface(CoreGameEngine engine)
        {
            Instance = new CoreGameEngineInterface(engine);
        }

        public void SendTextOutput(string s)
        {
            m_engine.SendTextOutput(s);
        }

        public void DamageTarget(ICharacterCore attacker, int damage, ICharacterCore target)
        {
            m_engine.CombatEngine.DamageTarget((Character)attacker, damage, (Character)target);
        }

        public bool Wait(ICharacterCore c)
        {
            return m_engine.Wait((Character)c);
        }

        public bool Move(ICharacterCore c, Direction direction)
        {
            return m_engine.Move((Character)c, direction);
        }

        public bool Attack(ICharacterCore attacker, Point attackTarget)
        {
            return m_engine.Attack((Character)attacker, attackTarget);
        }

        public bool Operate(ICharacterCore characterOperating, Point pointToOperateAt)
        {
            return m_engine.Operate((Character)characterOperating, pointToOperateAt);
        }

        public void MonsterNoticeRangedAttack(ICharacterCore monster, Utilities.Point positionAttackCameFrom)
        {
            ((Monster)monster).NoticeRangedAttack(positionAttackCameFrom);
        }

        public int GetMonsterVisionBonus()
        {
            return CombatDefenseCalculator.GetMonsterVisionBonus(m_engine.Player);
        }

        public ICharacterCore FindTargetAtPosition(Utilities.Point attackTarget)
        {
            return m_engine.CombatEngine.FindTargetAtPosition(attackTarget);
        }

        public List<Point> PathToPoint(ICharacterCore actor, Point dest, bool canOperate, bool usePlayerLOS, bool monstersBlockPath)
        {
            return m_engine.PathToPoint((Character)actor, dest, canOperate, usePlayerLOS, monstersBlockPath);
        }

        public List<ICharacterCore> MonstersInCharactersLOS(ICharacterCore character)
        {
            return m_engine.MonstersInCharactersLOS((Character)character).OfType<ICharacterCore>().ToList();
        }

        public bool[,] CalculateMoveablePointGrid(IMapCore map, bool monstersBlockPath)
        {
            return PhysicsEngine.CalculateMoveablePointGrid((Map)map, monstersBlockPath);
        }

        public ICharacterCore Player
        {
            get 
            {
                return m_engine.Player;
            }
        }

        public IMapCore Map
        {
            get
            {
                return m_engine.Map;
            }
        }

        public IFOVManager FOVManager
        {
            get
            {
                return m_engine.FOVManager;
            }
        }

        public IMonsterSkillEngine MonsterSkillEngine
        {
            get
            {
                return m_engine.MonsterSkillEngine;
            }
        }

        public ITreasureGenerator TreasureGenernator
        {
            get
            {
                return TreasureGenerator.Instance;
            }
        }
    }
}
