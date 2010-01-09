using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    enum SkillType { Rush }

    internal sealed class SkillEffectEngine
    {
        private PhysicsEngine m_physicsEngine;
        private CombatEngine m_combatEngine;

        internal SkillEffectEngine(PhysicsEngine physicsEngine, CombatEngine combatEngine)
        {
            m_physicsEngine = physicsEngine;
            m_combatEngine = combatEngine;
        }

        internal bool UseSkill(Character attacker, SkillType skill, Point target)
        {
            switch (skill)
            {
                case SkillType.Rush:
                    return HandleRush(attacker, skill, target);
                default:
                    return false;
            }
        }

        private bool HandleRush(Character attacker, SkillType skill, Point target)
        {
            // First the distance between us and target must be 2.
            List<Point> pathToPoint = CoreGameEngine.Instance.PathToPoint(attacker, target, false, false, true);
            if (pathToPoint.Count != 2)
                return false;

            // Second there must be a valid target
            Character targetCharacter = m_combatEngine.FindTargetAtPosition(target);
            if (targetCharacter == null)
                return false;

            // If we get here, it's a valid rush. Move towards target and attack at reduced time cost.
            CoreGameEngine.Instance.SendTextOutput(String.Format("{0} rushes towards {1} and attacks.", attacker.Name, targetCharacter.Name));
            m_physicsEngine.Move(attacker, PointDirectionUtils.ConvertTwoPointsToDirection(attacker.Position, pathToPoint[0]));
            m_combatEngine.Attack(attacker, target);

            return true;
        }
    }
}
