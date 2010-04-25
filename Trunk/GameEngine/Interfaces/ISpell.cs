using System;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface ISpell : INamedItem
    {
        string Name
        {
            get;
        }

        string School
        {
            get;
        }

        TargetingInfo Targeting
        {
            get;
        }
    }
}
