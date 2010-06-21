using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class HealerMonster : Monster
    {
        private int m_firstAidCooldown;

        public HealerMonster(string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
            m_firstAidCooldown = 0;
        }

        private bool CanUseFirstAid()
        {
            return m_firstAidCooldown == 0;
        }

        private void UsedFirstAid()
        {
            const int FirstAidCooldown = 4;
            m_firstAidCooldown = FirstAidCooldown;
        }

        private void NewTurn()
        {
            if (m_firstAidCooldown > 0)
                m_firstAidCooldown--;
        }

        public override void Action(CoreGameEngine engine)
        {
            NewTurn();
            List<ICharacter> nearbyAllies = OtherNearbyEnemies(engine);
            if (nearbyAllies.Count > 0 && CanUseFirstAid())
            {
                foreach (ICharacter allyNeedingHealing in nearbyAllies.Where(x => x.CurrentHP < x.MaxHP).OrderBy(x => x.CurrentHP))
                {
                    List<Point> pathToAlly = GetPathToCharacter(engine, allyNeedingHealing);
                    if (pathToAlly != null)
                    {
                        if (pathToAlly.Count <= 1)
                        {
                            bool sucessful = engine.UseSkill(this, SkillType.FirstAid, allyNeedingHealing.Position);
                            if (sucessful)
                            {
                                UsedFirstAid();
                                return;
                            }
                        }
                        else
                        {
                            bool sucessful = MoveOnPath(engine, pathToAlly);
                            if (sucessful)
                                return;
                        }
                    }
                }
            }

            DefaultAction(engine);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_firstAidCooldown = reader.ReadElementContentAsInt();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("FirstAidCooldown", m_firstAidCooldown.ToString());
        }
    }
}
