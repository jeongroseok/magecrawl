using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    // So in the current archtecture, each public method should do the action requested,
    // and _then_ call the CoreTimingEngine somehow to let others have their time slice before returnning
    // This is very synchronous, but easy to do.
    public sealed class CoreGameEngine : IDisposable
    {
        private const int MapWidth = 40;
        private const int MapHeight = 5;
        private Player m_player;
        private Map m_map;
        private SaveLoadCore m_saveLoad;
        private FOVManager m_fovManager;
        private PathfindingMap m_pathFinding;
        private CoreTimingEngine m_timingEngine;

        public CoreGameEngine()
        {
            m_player = new Player(1, 1);
            m_map = new Map(MapWidth, MapHeight);
            m_saveLoad = new SaveLoadCore();
            m_fovManager = new FOVManager(this);
            m_pathFinding = new PathfindingMap(m_fovManager);
            m_timingEngine = new CoreTimingEngine();

            // If the player isn't the first actor, let others go. See archtecture above.
            AfterPlayerAction();
        }

        public void Dispose()
        {
            m_fovManager.Dispose();
            m_pathFinding.Dispose();
        }

        internal void SetWithSaveData(Player p, Map m)
        {
            m_player = p;
            m_map = m;
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

        internal bool Move(Character c, Direction direction)
        {
            bool didAnything = false;
            Point newPosition = ConvertDirectionToDestinationPoint(c.Position, direction);
            if (IsPointOnMap(newPosition) && IsMovablePoint(newPosition))
            {
                c.Position = newPosition;
                m_timingEngine.ActorMadeMove(c);
                m_fovManager.Update(this);    // Operating can change LOS and such
                didAnything = true;
            }
            return didAnything;
        }

        public bool MovePlayer(Direction direction)
        {
            bool didAnything = Move(m_player, direction);

            if (didAnything)
                AfterPlayerAction();
            return didAnything;
        }

        public bool Operate(Direction direction)
        {
            bool didAnything = false;

            Point newPosition = ConvertDirectionToDestinationPoint(m_player.Position, direction);
            foreach (MapObject obj in Map.MapObjects)
            {
                OperableMapObject operateObj = obj as OperableMapObject;
                if (operateObj != null && operateObj.Position == newPosition)
                {
                    operateObj.Operate();
                    m_timingEngine.ActorDidAction(m_player);
                    m_fovManager.Update(this);    // Operating can change LOS and such
                    didAnything = true;
                }
            }
            if (didAnything)
                AfterPlayerAction();
            return didAnything;
        }

        internal bool Wait(Character c)
        {
            m_timingEngine.ActorDidAction(c);
            return true;
        }

        public bool PlayerWait()
        {
            Wait(m_player);
            AfterPlayerAction();
            return true;
        }

        public void Save()
        {
            m_saveLoad.SaveGame(this);
        }

        public void Load()
        {
            m_saveLoad.LoadGame(this);
            m_fovManager.Update(this);    // We have a new map, recalc LOS
        }

        public IList<Point> PlayerPathToPoint(Point dest)
        {
            return m_pathFinding.Travel(m_player.Position, dest);
        }

        internal IList<Point> PathToPoint(Point source, Point dest)
        {
            return m_pathFinding.Travel(source, dest);
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

        internal static Direction ConvertTwoPointsToDirection(Point initial, Point end)
        {
            int x = end.X - initial.X;
            int y = end.Y - initial.Y;
            if (x > 1)
                x = 1;
            if (x < -1)
                x = -1;
            if (y > 1)
                y = 1;
            if (y < -1)
                y = -1;
            return ConvertPositionDeltaToDirection(x, y);
        }

        private static Direction ConvertPositionDeltaToDirection(int deltaX, int deltaY)
        {
            if (deltaX == 1)
            {
                if (deltaY == 1)
                    return Direction.Southeast;
                else if (deltaY == -1)
                    return Direction.Northeast;
                else
                    return Direction.East;
            }
            else if (deltaX == -1)
            {
                if (deltaY == 1)
                    return Direction.Southwest;
                else if (deltaY == -1)
                    return Direction.Northwest;
                else
                    return Direction.West;
            }
            else
            {
                if (deltaY == 1)
                    return Direction.South;
                else if (deltaY == -1)
                    return Direction.North;
                else
                    throw new System.ArgumentOutOfRangeException("ConvertPositionDeltaToDirection - No direction?");
            }
        }

        private static bool IsPointOnMap(Point p)
        {
            return (p.X >= 0) && (p.Y >= 0) && (p.X < MapWidth) && (p.Y < MapHeight);
        }

        private void AfterPlayerAction()
        {
            Character nextCharacter = m_timingEngine.GetNextActor(this);
            if (nextCharacter is Player)
                return;
            
            if (!(nextCharacter is Monster))
                throw new System.NotSupportedException("Any character that isn't the player should be a monster");
            
            Monster monster = nextCharacter as Monster;
            MonsterAction result = monster.Action(this);

            // We don't need to cost moves, since move itself does so already
            // TODO - Make all actions cost, and remove this
            if (result == MonsterAction.DidAction)
                m_timingEngine.ActorDidAction(monster);
        }

        // Moveable wants player point ot be closed (Monsters don't move into player)
        internal bool IsMovablePoint(Point p)
        {
            bool isMoveablePoint = IsPathablePoint(p);

            if (m_player.Position == p)
                isMoveablePoint = false;

            return isMoveablePoint;
        }

        // Pathfinding wants player point to be open (Monster Pathfinding)
        internal bool IsPathablePoint(Point p)
        {
            bool isPathablePoint = m_map[p.X, p.Y].Terrain == Magecrawl.GameEngine.Interfaces.TerrainType.Floor;

            foreach (MapObject obj in m_map.MapObjects)
            {
                if (obj.Position == p && obj.IsSolid)
                    isPathablePoint = false;
            }

            foreach (Monster m in m_map.Monsters)
            {
                if (m.Position == p)
                    isPathablePoint = false;
            }

            return isPathablePoint;
        }
    }
}
