using System;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface ISpell : INamedItem
    {
        string TargetType
        {
            get;
        }

        string School
        {
            get;
        }
    }
}
