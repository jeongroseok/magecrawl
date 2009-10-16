using System;
using System.Collections.Generic;
using Utilities;

namespace GameEngine
{
    public sealed class CoreGameEngine
    {
        private const int MapWidth = 60;
        private const int MapHeight = 40;
        private Player m_player;
        private Map m_map;

        public CoreGameEngine()
        {
            m_player = new Player(1, 1);
            m_map = new Map(MapWidth, MapHeight);
        }

        public Interfaces.IPlayer Player
        {
            get
            {
                return m_player;
            }
        }

        public Interfaces.IMap Map
        {
            get
            {
                return m_map;
            }
        }

        public void MovePlayer(MovementDirection direction)
        {
            Point newPosition = ConvertDirectionToDestinationPoint(m_player.Position, direction);
            if (IsPointOnMap(newPosition) && IsMovablePoint(newPosition))
                m_player.Position = newPosition;
        }

        private static Point ConvertDirectionToDestinationPoint(Point initial, MovementDirection direction)
        {
            Point destPoint;
            switch (direction)
            {
                case MovementDirection.North:
                    destPoint = new Point(initial.X, initial.Y - 1);
                    break;
                case MovementDirection.South:
                    destPoint = new Point(initial.X, initial.Y + 1);
                    break;
                case MovementDirection.West:
                    destPoint = new Point(initial.X - 1, initial.Y);
                    break;
                case MovementDirection.East:
                    destPoint = new Point(initial.X + 1, initial.Y);
                    break;
                case MovementDirection.Northeast:
                    destPoint = new Point(initial.X + 1, initial.Y - 1);
                    break;
                case MovementDirection.Northwest:
                    destPoint = new Point(initial.X - 1, initial.Y - 1);
                    break;
                case MovementDirection.Southeast:
                    destPoint = new Point(initial.X + 1, initial.Y + 1);
                    break;
                case MovementDirection.Southwest:
                    destPoint = new Point(initial.X - 1, initial.Y + 1);
                    break;
                default:
                    throw new ArgumentException("ConvertDirectionToDestinationPoint - Invalid Direction");
            }
            return destPoint;
        }

        private static bool IsPointOnMap(Point p)
        {
            return (p.X >= 0) && (p.Y >= 0) && (p.X < MapWidth) && (p.Y < MapHeight);
        }

        private bool IsMovablePoint(Point p)
        {
            return m_map[p.X, p.Y].Terrain == GameEngine.Interfaces.TerrainType.Floor;
        }
    }
}
