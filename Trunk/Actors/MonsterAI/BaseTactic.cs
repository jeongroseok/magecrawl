using System.Collections.Generic;
using System.Linq;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Actors.MonsterAI
{
    internal abstract class BaseTactic : IMonsterTactic
    {
        public abstract bool CouldUseTactic(IGameEngineCore engine, Monster monster);
        public abstract bool UseTactic(IGameEngineCore engine, Monster monster);
        public virtual void SetupAttributesNeeded(Monster monster) 
        { 
        }

        public virtual void NewTurn(Monster monster)
        {
        }

        public abstract bool NeedsPlayerLOS { get; }

        protected bool WalkTowardsLastKnownPosition(IGameEngineCore engine, Monster monster)
        {
            if (monster.PlayerLastKnownPosition == Point.Invalid)
                return false;

            List<Point> pathTowards = engine.PathToPoint(monster, monster.PlayerLastKnownPosition, monster.Intelligent, false, true);
            if (pathTowards == null || pathTowards.Count == 0)
            {
                monster.PlayerLastKnownPosition = Point.Invalid;
                return false;
            }
            else
            {
                return MoveOnPath(engine, pathTowards, monster);
            }
        }

        protected List<Point> GetPathToCharacter(IGameEngineCore engine, Monster monster, ICharacter c)
        {
            return engine.PathToPoint(monster, c.Position, monster.Intelligent, false, true);
        }

        protected List<Point> GetPathToPlayer(IGameEngineCore engine, Monster monster)
        {
            return engine.PathToPoint(monster, engine.Player.Position, monster.Intelligent, false, true);
        }

        protected bool IsNextToPlayer(IGameEngineCore engine, Monster monster)
        {
            return GetPathToPlayerLength(engine, monster) == 1;
        }

        protected bool IsNextToPlayer(IGameEngineCore engine, List<Point> pathToPlayer)
        {
            return pathToPlayer != null && pathToPlayer.Count == 1;
        }

        protected int GetPathToPlayerLength(IGameEngineCore engine, Monster monster)
        {
            List<Point> pathToPlayer = GetPathToPlayer(engine, monster);
            if (pathToPlayer == null)
                return -1;
            return pathToPlayer.Count;
        }

        protected bool HasPathToPlayer(IGameEngineCore engine, List<Point> pathToPlayer)
        {
            return pathToPlayer != null && pathToPlayer.Count > 0;
        }

        protected bool AttackPlayer(IGameEngineCore engine, Monster monster)
        {
            if (engine.Attack(monster, engine.Player.Position))
                return true;
            return false;
        }

        protected bool MoveTowardsPlayer(IGameEngineCore engine, Monster monster)
        {
            return MoveOnPath(engine, GetPathToPlayer(engine, monster), monster);
        }

        private bool MoveCore(IGameEngineCore engine, Direction direction, Monster monster)
        {
            if (engine.Move(monster, direction))
                return true;
            Point position = PointDirectionUtils.ConvertDirectionToDestinationPoint(monster.Position, direction);
            if (monster.Intelligent && engine.Operate(monster, position))
                return true;
            return false;
        }

        protected bool MoveOnPath(IGameEngineCore engine, List<Point> path, Monster monster)
        {
            Direction nextPosition = PointDirectionUtils.ConvertTwoPointsToDirection(monster.Position, path[0]);
            return MoveCore(engine, nextPosition, monster);
        }

        protected bool MoveNearbyOnPath(IGameEngineCore engine, List<Point> path, Monster monster)
        {
            Direction nextPosition = PointDirectionUtils.GetDirectionsNearby(PointDirectionUtils.ConvertTwoPointsToDirection(monster.Position, path[0]))[0];
            return MoveCore(engine, nextPosition, monster);
        }

        protected bool MoveAwayFromPlayer(IGameEngineCore engine, Monster monster)
        {
            Direction directionTowardsPlayer = PointDirectionUtils.ConvertTwoPointsToDirection(monster.Position, engine.Player.Position);
            if (MoveCore(engine, PointDirectionUtils.GetDirectionOpposite(directionTowardsPlayer), monster))
                return true;

            foreach (Direction attemptDirection in PointDirectionUtils.GetDirectionsOpposite(directionTowardsPlayer))
            {
                if (MoveCore(engine, attemptDirection, monster))
                    return true;
            }
            return false;
        }

        protected List<Character> OtherNearbyEnemies(IGameEngineCore engine, Monster monster)
        {
            return engine.MonstersInCharactersLOS(monster).Where(x => PointDirectionUtils.NormalDistance(x.Position, engine.Player.Position) < 4).OfType < Character>().ToList();
        }

        protected bool AreOtherNearbyEnemies(IGameEngineCore engine, Monster monster)
        {
            return OtherNearbyEnemies(engine, monster).Count() > 1;
        }

        protected bool IsPlayerVisible(IGameEngineCore engine, Monster monster)
        {
            return engine.FOVManager.VisibleSingleShot(engine.Map, monster.Position, monster.Vision, engine.Player.Position);
        }

        protected bool WanderRandomly(IGameEngineCore engine, Monster monster)
        {
            foreach (Direction d in DirectionUtils.GenerateRandomDirectionList())
            {
                if (engine.Move(monster, d))
                    return true;
            }

            // If nothing else, 'wait'
            engine.Wait(monster);
            return false;
        }
    }
}
