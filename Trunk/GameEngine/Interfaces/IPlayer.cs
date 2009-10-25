using System;
using Magecrawl.Utilities;
using System.Collections.Generic;

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
