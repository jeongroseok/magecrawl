using System;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface ISpell : INamedItem
    {
        bool NeedTarget
        {
            get;
        }
    }
}
