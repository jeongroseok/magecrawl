using System.Collections.Generic;
using System.Linq;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects
{
    internal class EffectEngine
    {
        private CombatEngine m_combatEngine;
        private PhysicsEngine m_physicsEngine;

        internal EffectEngine(PhysicsEngine physicsEngine, CombatEngine combatEngine)
        {
            m_combatEngine = combatEngine;
            m_physicsEngine = physicsEngine;
        }

        internal bool AddEffectToTarget(string effectName, Character invoker, int strength, bool longTerm, Point target)
        {
            return AddEffectToTarget(effectName, invoker, strength, longTerm, target, null);
        }

        internal bool AddEffectToTarget(string effectName, Character invoker, int strength, bool longTerm, Point target, string toPrintOnEffectAdd)
        {
            Character targetCharacter = m_combatEngine.FindTargetAtPosition(target);
            if (targetCharacter != null)
            {
                bool successOnAddEffect;
                if (longTerm)
                    successOnAddEffect = HandleLongTermEffect(effectName, invoker, strength);
                else
                    successOnAddEffect = HandleShortTermEffect(effectName, invoker, strength, targetCharacter);
                if (successOnAddEffect && toPrintOnEffectAdd != null)
                    CoreGameEngine.Instance.SendTextOutput(toPrintOnEffectAdd);
                return true;
            }
            return false;
        }

        private static bool HandleShortTermEffect(string effectName, Character invoker, int strength, Character targetCharacter)
        {
            DismissExistingEffect(effectName, targetCharacter);

            StatusEffect effect = EffectFactory.CreateEffect(invoker, effectName, false, strength);
            targetCharacter.AddEffect(effect);
            if (targetCharacter is Monster && invoker is Player && !effect.IsPositiveEffect)
                ((Monster)targetCharacter).NoticeRangedAttack(invoker.Position);
            return true;
        }

        private static bool HandleLongTermEffect(string effectName, Character invoker, int strength)
        {
            DismissExistingEffect(effectName, invoker);

            // Check here if mp will bring us under 0 since mp cost of spell hasn't hit yet...
            invoker.AddEffect(EffectFactory.CreateEffect(invoker, effectName, true, strength));
            return true;           
        }

        private static void DismissExistingEffect(string effectName, Character target)
        {
            List<StatusEffect> statusList = target.Effects.Where(s => s.Name == effectName).ToList();
            if (statusList.Count > 1)
                throw new System.InvalidOperationException("DismissExistingEffect with more than one effect of the same name on them?");
            foreach (StatusEffect s in statusList)
            {
                CoreGameEngine.Instance.SendTextOutput(string.Format("The existing {0} effect fades from {1}.", effectName, target.Name));
                s.Dismiss();
            }
        }
    }
}
