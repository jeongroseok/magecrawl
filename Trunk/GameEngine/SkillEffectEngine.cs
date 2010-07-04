using System;
using System.Collections.Generic;
using System.Reflection;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal enum SkillType
    {
        Rush,
        DoubleSwing,
        FirstAid
    }

    internal sealed class SkillEffectEngine
    {
        private PhysicsEngine m_physicsEngine;
        private CombatEngine m_combatEngine;

        internal SkillEffectEngine(PhysicsEngine physicsEngine, CombatEngine combatEngine)
        {
            m_physicsEngine = physicsEngine;
            m_combatEngine = combatEngine;
        }

        internal bool UseSkill(Character invoker, SkillType skill, Point target)
        {
            // Find the method implementing the skill
            MethodInfo skillMethod = GetType().GetMethod("Handle" + skill.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);

            // And invoke it
            return (bool)skillMethod.Invoke(this, new object[] { invoker, skill, target });
        }

        private bool HandleFirstAid(Character invoker, SkillType skill, Point target)
        {
            Character targetCharacter = ValidTargetLessThanOrEqualTo(invoker, target, 1);
            if (targetCharacter == null)
                return false;

            // If we get here, it's a valid first aid. Increase target's HP by amount
            string targetString = targetCharacter == invoker ? "themself" : "the " + targetCharacter.Name;
            CoreGameEngine.Instance.SendTextOutput(String.Format("The {0} applies some fast combat medicine on {1}.", invoker.Name, targetString));
            int amountToHeal = (new DiceRoll(1, 4, 1, 1)).Roll();
            targetCharacter.Heal(amountToHeal, false);
            CoreGameEngine.Instance.Wait(invoker);
            return true;
        }

        private bool HandleDoubleSwing(Character attacker, SkillType skill, Point target)
        {
            Character targetCharacter = ValidTarget(attacker, target, 1);
            if (targetCharacter == null)
                return false;

            // If we get here, it's a valid double swing. Attack two times in a row.
            CoreGameEngine.Instance.SendTextOutput(String.Format("{0} wildly swings at {1} twice.", attacker.Name, targetCharacter.Name));           
            m_physicsEngine.Attack(attacker, target);
            m_physicsEngine.Attack(attacker, target);

            return true;
        }

        private bool HandleRush(Character attacker, SkillType skill, Point target)
        {
            Character targetCharacter = ValidTarget(attacker, target, 2);
            if (targetCharacter == null)
                return false;

            // If we get here, it's a valid rush. Move towards target and attack at reduced time cost.
            CoreGameEngine.Instance.SendTextOutput(String.Format("{0} rushes towards {1} and attacks.", attacker.Name, targetCharacter.Name));
            List<Point> pathToPoint = CoreGameEngine.Instance.PathToPoint(attacker, target, false, false, true);
            m_physicsEngine.Move(attacker, PointDirectionUtils.ConvertTwoPointsToDirection(attacker.Position, pathToPoint[0]));
            m_physicsEngine.Attack(attacker, target);

            return true;
        }

        private Character ValidTarget(Character attacker, Point targetSquare, int requiredDistance)
        {
            // First the distance between us and target must be requiredDistance.
            List<Point> pathToPoint = CoreGameEngine.Instance.PathToPoint(attacker, targetSquare, false, false, true);
            if (pathToPoint.Count != requiredDistance)
                return null;

            return m_combatEngine.FindTargetAtPosition(targetSquare);            
        }

        private Character ValidTargetLessThanOrEqualTo(Character attacker, Point targetSquare, int requiredDistance)
        {
            // First the distance between us and target must be requiredDistance.
            List<Point> pathToPoint = CoreGameEngine.Instance.PathToPoint(attacker, targetSquare, false, false, true);
            if (pathToPoint.Count > requiredDistance)
                return null;

            return m_combatEngine.FindTargetAtPosition(targetSquare);
        }
    }
}
