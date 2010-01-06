using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IPlayer : ICharacter
    {
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
    }
}
