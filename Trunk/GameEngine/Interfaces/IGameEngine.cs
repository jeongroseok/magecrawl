using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IGameEngine : IDisposable
    {
        IPlayer Player
        {
            get;
        }

        IMap Map
        {
            get;
        }

        bool MovePlayer(Direction direction);
        bool Operate(Direction direction);
        bool PlayerWait();
        void Save();
        void Load();
        bool PlayerAttack(Direction direction);
        IList<Point> PlayerPathToPoint(Point dest);
    }
}
