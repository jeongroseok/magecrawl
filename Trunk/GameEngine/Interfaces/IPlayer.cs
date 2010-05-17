using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IPlayer : ICharacter
    {
        int CurrentMP
        {
            get;
        }

        int MaxMP
        {
            get;
        }

        int SkillPoints
        {
            get;
        }

        IEnumerable<IItem> Items
        {
            get;
        }

        IEnumerable<ISpell> Spells
        {
            get;
        }

        IEnumerable<ISkill> Skills
        {
            get;
        }

        IEnumerable<string> StatusEffects
        {
            get;
        }

        IArmor ChestArmor
        {
            get;
        }

        IArmor Headpiece
        {
            get;
        }
        
        IArmor Gloves
        {
            get;
        }

        IArmor Boots
        {
            get;
        }
    }
}
