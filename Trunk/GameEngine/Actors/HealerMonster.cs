using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class HealerMonster : Monster
    {
        public HealerMonster(string name, Point p, int maxHP, bool intelligent, int vision, DiceRoll damage, double evade, double ctIncreaseModifer, double ctMoveCost, double ctActCost, double ctAttackCost)
            : base(name, p, maxHP, intelligent, vision, damage, evade, ctIncreaseModifer, ctMoveCost, ctActCost, ctAttackCost)
        {
            Attributes["FirstAidCooldown"] = "0";
        }

        private bool CanUseFirstAid()
        {
            return Attributes["FirstAidCooldown"] == "0";
        }

        private void UsedFirstAid()
        {
            const int FirstAidCooldown = 4;
            Attributes["FirstAidCooldown"] = FirstAidCooldown.ToString();
        }

        private void NewTurn()
        {
            int currentValue = int.Parse(Attributes["FirstAidCooldown"]);
            if (currentValue > 0)
                Attributes["FirstAidCooldown"] = (currentValue - 1).ToString();
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
                            bool sucessful = engine.UseMonsterSkill(this, SkillType.FirstAid, allyNeedingHealing.Position);
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
    }
}
