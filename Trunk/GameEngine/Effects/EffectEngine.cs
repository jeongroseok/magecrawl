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

        internal bool AddEffectToTarget(string effectName, Character invoker, int strength, bool longTerm, Point target, int mpTotalAfterSpellCastIfApplicable)
        {
            return AddEffectToTarget(effectName, invoker, strength, longTerm, target, mpTotalAfterSpellCastIfApplicable, null);
        }

        internal bool AddEffectToTarget(string effectName, Character invoker, int strength, bool longTerm, Point target, int mpTotalAfterSpellCastIfApplicable, string toPrintOnEffectAdd)
        {
            Character targetCharacter = m_combatEngine.FindTargetAtPosition(target);
            if (targetCharacter != null)
            {
                bool successOnAddEffect;
                if (longTerm)
                    successOnAddEffect = HandleLongTermEffect(effectName, invoker, strength, mpTotalAfterSpellCastIfApplicable);
                else
                    successOnAddEffect = HandleShortTermEffect(effectName, invoker, strength, longTerm, targetCharacter);
                if (successOnAddEffect && toPrintOnEffectAdd != null)
                    CoreGameEngine.Instance.SendTextOutput(toPrintOnEffectAdd);
                return true;
            }
            return false;
        }

        private static bool HandleShortTermEffect(string effectName, Character invoker, int strength, bool longTerm, Character targetCharacter)
        {
            DismissExistingEffect(effectName, invoker, targetCharacter);

            StatusEffect effect = EffectFactory.CreateEffect(invoker, effectName, longTerm, strength);
            targetCharacter.AddEffect(effect);
            if (targetCharacter is Monster && invoker is Player && !effect.IsPositiveEffect)
                ((Monster)targetCharacter).NoticeRangedAttack(invoker.Position);
            return true;
        }

        private static bool HandleLongTermEffect(string effectName, Character invoker, int strength, int mpTotalAfterSpellCastIfApplicable)
        {
            DismissExistingEffect(effectName, invoker, invoker);

            LongTermEffect effect = (LongTermEffect)EffectFactory.CreateEffect(invoker, effectName, true, strength);
            Player invokerAsPlayer = invoker as Player;
            if (invokerAsPlayer != null)
            {
                if (effect.MPCost > mpTotalAfterSpellCastIfApplicable && mpTotalAfterSpellCastIfApplicable != -1)
                {
                    CoreGameEngine.Instance.SendTextOutput(string.Format("Cannot cast {0} as {1} does not have the energy to sustain it.", effectName, invoker.Name));
                    return false;
                }
            }
            // Check here if mp will bring us under 0 since mp cost of spell hasn't hit yet...
            invoker.AddEffect(effect);
            return true;           
        }

        private static void DismissExistingEffect(string effectName, Character invoker, Character target)
        {
            List<StatusEffect> statusList = invoker.Effects.Where(s => s.Name == effectName).ToList();
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
