using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public delegate void PlayerDiedDelegate();

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

        Point TargetSelection
        {
            get;
            set;
        }

        bool SelectingTarget
        {
            get;
            set;
        }

        bool MovePlayer(Direction direction);
        bool Operate(Direction direction);
        bool PlayerWait();
        void Save();
        void Load();
        bool PlayerAttack(Direction direction);
        bool PlayerAttack(Point target);
        IList<Point> PlayerPathToPoint(Point dest);
        bool[,] PlayerMoveableToEveryPoint();
    }
}
