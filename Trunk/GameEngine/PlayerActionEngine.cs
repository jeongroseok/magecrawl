using System.Linq;
using Magecrawl.GameEngine.Effects;
using Magecrawl.GameEngine.Magic;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal class PlayerActionEngine : IEngineActions
    {
        private CoreGameEngine m_engine;

        internal PlayerActionEngine(CoreGameEngine engine)
        {
            m_engine = engine;
        }

        public bool Move(Direction direction)
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.Move(m_engine.Player, direction);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool Operate(Point pointToOperateAt)
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.Operate(m_engine.Player, pointToOperateAt);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool Wait()
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.Wait(m_engine.Player);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool Attack(Point target)
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.Attack(m_engine.Player, target);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool ReloadWeapon()
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.ReloadWeapon(m_engine.Player);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool CastSpell(ISpell spell, Point target)
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.CastSpell(m_engine.Player, (Spell)spell, target);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool GetItem()
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.PlayerGetItem();
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool GetItem(IItem item)
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.PlayerGetItem(item);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool MoveDownStairs()
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.PlayerMoveDownStairs();
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool MoveUpStairs()
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.PlayerMoveUpStairs();
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public void AddSkillToPlayer(ISkill skill)
        {
            if (m_engine.Player.SkillPoints < skill.Cost)
                throw new System.InvalidOperationException("AddSkillToPlayer without enough SP");
            m_engine.Player.SkillPoints -= skill.Cost;
            m_engine.Player.AddSkill(skill);
        }

        public void DismissEffect(string effectName)
        {
            StatusEffect effectInQuestion = m_engine.Player.Effects.FirstOrDefault(e => e.DisplayName == effectName);
            if (effectInQuestion == null)
                throw new System.InvalidOperationException("Trying to DismissEffect " + effectName + " and can not find.");
            if (!effectInQuestion.IsPositiveEffect)
                throw new System.InvalidOperationException("Trying to DismissEffect a non-positive effect");
            m_engine.BeforePlayerAction();
            effectInQuestion.Dismiss();
            m_engine.Wait(m_engine.Player); // Waiting passes time, which we want dismissing effects to take
            m_engine.AfterPlayerAction();
        }

        public bool SwapPrimarySecondaryWeapons()
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.SwapPrimarySecondaryWeapons(m_engine.Player);
            if (didAnything)
            {
                m_engine.SendTextOutput("Weapons Swapped");
                m_engine.AfterPlayerAction();
            }
            return didAnything;
        }

        public bool SelectedItemOption(IItem item, string option, object argument)
        {
            m_engine.BeforePlayerAction();
            bool didAnything = m_engine.PlayerSelectedItemOption(item, option, argument);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }
    }
}
