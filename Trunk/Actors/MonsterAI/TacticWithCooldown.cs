using System.Globalization;

namespace Magecrawl.Actors.MonsterAI
{
    internal abstract class TacticWithCooldown : BaseTactic
    {
        protected void SetupAttribute(Monster monster, string attribute, string value)
        {
            monster.Attributes[attribute] = value;
        }

        protected bool CanUseCooldown(Monster monster, string cooldownAttributes)
        {
            return monster.Attributes[cooldownAttributes] == "0";
        }

        protected void UsedCooldown(Monster monster, string cooldownAttributes, int cooldownAmount)
        {
            monster.Attributes[cooldownAttributes] = cooldownAmount.ToString();
        }

        protected void NewTurn(Monster monster, string cooldownAttributes)
        {
            int currentValue = int.Parse(monster.Attributes[cooldownAttributes], CultureInfo.InvariantCulture);
            if (currentValue > 0)
                monster.Attributes[cooldownAttributes] = (currentValue - 1).ToString();
        }
    }
}
