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

        IList<IItem> Items
        {
            get;
        }

        IList<ISpell> Spells
        {
            get;
        }

        IList<string> StatusEffects
        {
            get;
        }

        IArmor ChestArmor
        {
            get;
        }
    }
}
