using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IPlayer : ICharacter
    {
        string Name
        {
            get;
        }

        IList<IItem> Items
        {
            get;
        }
    }
}
