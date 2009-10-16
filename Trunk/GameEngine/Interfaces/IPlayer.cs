using System;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IPlayer
    {
        Point Position
        {
            get;
        }

        string Name
        {
            get;
        }
    }
}
