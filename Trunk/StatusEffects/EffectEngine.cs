using System.Collections.Generic;
using System.Linq;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.StatusEffects
{
    public class EffectEngine
    {
        private IGameEngineCore m_engine;

        public EffectEngine(IGameEngineCore engine)
        {
            m_engine = engine;
        }

        public bool AddEffectToTarget(string effectName, ICharacterCore invoker, int strength, bool longTerm, Point target)
        {
            return AddEffectToTarget(effectName, invoker, strength, longTerm, target, null);
        }

        public bool AddEffectToTarget(string effectName, ICharacterCore invoker, int strength, bool longTerm, Point target, string toPrintOnEffectAdd)
        {
            ICharacterCore targetCharacter = m_engine.FindTargetAtPosition(target);
            if (targetCharacter != null)
            {
                bool successOnAddEffect;
                if (longTerm)
                    successOnAddEffect = HandleLongTermEffect(effectName, invoker, strength);
                else
                    successOnAddEffect = HandleShortTermEffect(effectName, invoker, strength, targetCharacter);
                if (successOnAddEffect && toPrintOnEffectAdd != null)
                    m_engine.SendTextOutput(toPrintOnEffectAdd);
                return true;
            }
            return false;
        }

        private bool HandleShortTermEffect(string effectName, ICharacterCore invoker, int strength, ICharacterCore targetCharacter)
        {
            DismissExistingEffect(effectName, targetCharacter);

            IStatusEffectCore effect = EffectFactory.CreateEffect(invoker, effectName, false, strength);
            targetCharacter.AddEffect(effect);
            if (targetCharacter is IMonster && invoker is IPlayer && !effect.IsPositiveEffect)
                m_engine.MonsterNoticeRangedAttack(targetCharacter, invoker.Position);
            return true;
        }

        private bool HandleLongTermEffect(string effectName, ICharacterCore invoker, int strength)
        {
            DismissExistingEffect(effectName, invoker);

            // Check here if mp will bring us under 0 since mp cost of spell hasn't hit yet...
            invoker.AddEffect(EffectFactory.CreateEffect(invoker, effectName, true, strength));
            return true;           
        }

        // For effects that mess with HPs on add/remove, this "works" for now since
        // We'll call Dismiss on the previous effect, which waits until end of turn to Remove
        // Then we'll call Add on the new effect, which'll add the HP first, then the remove will remove them
        // If we ever have an effect that is percentage based, we'll have to redo this...
        private void DismissExistingEffect(string effectName, ICharacterCore target)
        {
            List<IStatusEffectCore> statusList = target.Effects.Where(s => s.Type == effectName).ToList();
            if (statusList.Count > 1)
                throw new System.InvalidOperationException("DismissExistingEffect with more than one effect of the same name on them?");
            foreach (StatusEffect s in statusList)
            {
                m_engine.SendTextOutput(string.Format("The existing {0} effect fades from {1}.", s.Name, target.Name));
                s.Dismiss();
            }
        }
    }
}
