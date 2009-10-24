using System;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface ICharacter
    {
        Point Position
        {
            get;
        }

        int CurrentHP
        {
            get;
        }

        int MaxHP
        {
            get;
        }

        int Vision
        {
            get;
        }

        int UniqueID
        {
            get;
        }        
    }
}
