using System;
using System.Collections.Generic;
using GameEngine.Interfaces;
using GameEngine.MapObjects;
using Utilities;

namespace GameEngine
{
    public sealed class CoreGameEngine
    {
        private const int MapWidth = 40;
        private const int MapHeight = 3;
        private Player m_player;
        private Map m_map;

        public CoreGameEngine()
        {
            m_player = new Player(1, 1);
            m_map = new Map(MapWidth, MapHeight);
        }

        public IPlayer Player
        {
            get
            {
                return m_player;
            }
        }

        public IMap Map
        {
            get
            {
                return m_map;
            }
        }

        public void MovePlayer(Direction direction)
        {
            Point newPosition = ConvertDirectionToDestinationPoint(m_player.Position, direction);
            if (IsPointOnMap(newPosition) && IsMovablePoint(newPosition))
                m_player.Position = newPosition;
        }

        public void Operate(Direction direction)
        {
            Point newPosition = ConvertDirectionToDestinationPoint(m_player.Position, direction);
            foreach (MapObject obj in Map.MapObjects)
            {
                OperableMapObject operateObj = obj as OperableMapObject;
                if (operateObj != null)
                    operateObj.Operate();
            }
        }

        private static Point ConvertDirectionToDestinationPoint(Point initial, Direction direction)
        {
            Point destPoint;
            switch (direction)
            {
                case Direction.North:
                    destPoint = new Point(initial.X, initial.Y - 1);
                    break;
                case Direction.South:
                    destPoint = new Point(initial.X, initial.Y + 1);
                    break;
                case Direction.West:
                    destPoint = new Point(initial.X - 1, initial.Y);
                    break;
                case Direction.East:
                    destPoint = new Point(initial.X + 1, initial.Y);
                    break;
                case Direction.Northeast:
                    destPoint = new Point(initial.X + 1, initial.Y - 1);
                    break;
                case Direction.Northwest:
                    destPoint = new Point(initial.X - 1, initial.Y - 1);
                    break;
                case Direction.Southeast:
                    destPoint = new Point(initial.X + 1, initial.Y + 1);
                    break;
                case Direction.Southwest:
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
            bool isMovablePoint = m_map[p.X, p.Y].Terrain == GameEngine.Interfaces.TerrainType.Floor;

            foreach (MapObject obj in m_map.MapObjects)
            {
                if (obj.Position == p && obj.IsSolid)
                    isMovablePoint = false;
            }

            return isMovablePoint;
        }
    }
}
