using System;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface ISpell : INamedItem
    {
        string Name
        {
            get;
        }

        string TargetType
        {
            get;
        }

        string School
        {
            get;
        }

        int Range
        {
            get;
        }
    }
}
