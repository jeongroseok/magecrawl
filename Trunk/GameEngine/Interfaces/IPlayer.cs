using System;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IPlayer : ICharacter
    {
        string Name
        {
            get;
        }
    }
}
