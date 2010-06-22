using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public interface IEngineActions
    {
        bool Move(Direction direction);
        bool Operate(Point pointToOperateAt);
        bool Wait();
        bool GetItem();
        bool GetItem(IItem item);
        bool Attack(Point target);

        bool CastSpell(ISpell spell, Point target);
        bool ReloadWeapon();
        bool SwapPrimarySecondaryWeapons();

        bool MoveDownStairs();
        bool MoveUpStairs();

        void AddSkillToPlayer(ISkill skill);

        void DismissEffect(string name);

        bool SelectedItemOption(IItem item, string option, object argument);
    }
}
