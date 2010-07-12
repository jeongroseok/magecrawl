using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    internal abstract class BaseTactic : IMonsterTactic
    {
        public abstract bool CouldUseTactic(CoreGameEngine engine, Monster monster);
        public abstract bool UseTactic(CoreGameEngine engine, Monster monster);
        public virtual void SetupAttributesNeeded(Monster monster) 
        { 
        }

        public virtual void NewTurn(Monster monster)
        {
        }

        public abstract bool NeedsPlayerLOS { get; }

        protected bool WalkTowardsLastKnownPosition(CoreGameEngine engine, Monster monster)
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

        protected List<Point> GetPathToCharacter(CoreGameEngine engine, Monster monster, ICharacter c)
        {
            return engine.PathToPoint(monster, c.Position, monster.Intelligent, false, true);
        }

        protected List<Point> GetPathToPlayer(CoreGameEngine engine, Monster monster)
        {
            return engine.PathToPoint(monster, engine.Player.Position, monster.Intelligent, false, true);
        }

        protected bool IsNextToPlayer(CoreGameEngine engine, Monster monster)
        {
            return GetPathToPlayerLength(engine, monster) == 1;
        }

        protected bool IsNextToPlayer(CoreGameEngine engine, List<Point> pathToPlayer)
        {
            return pathToPlayer != null && pathToPlayer.Count == 1;
        }

        protected int GetPathToPlayerLength(CoreGameEngine engine, Monster monster)
        {
            List<Point> pathToPlayer = GetPathToPlayer(engine, monster);
            if (pathToPlayer == null)
                return -1;
            return pathToPlayer.Count;
        }

        protected bool HasPathToPlayer(CoreGameEngine engine, List<Point> pathToPlayer)
        {
            return pathToPlayer != null && pathToPlayer.Count > 0;
        }

        protected bool AttackPlayer(CoreGameEngine engine, Monster monster)
        {
            if (engine.Attack(monster, engine.Player.Position))
                return true;
            return false;
        }

        protected bool MoveTowardsPlayer(CoreGameEngine engine, Monster monster)
        {
            return MoveOnPath(engine, GetPathToPlayer(engine, monster), monster);
        }

        private bool MoveCore(CoreGameEngine engine, Direction direction, Monster monster)
        {
            if (engine.Move(monster, direction))
                return true;
            Point position = PointDirectionUtils.ConvertDirectionToDestinationPoint(monster.Position, direction);
            if (monster.Intelligent && engine.Operate(monster, position))
                return true;
            return false;
        }

        protected bool MoveOnPath(CoreGameEngine engine, List<Point> path, Monster monster)
        {
            Direction nextPosition = PointDirectionUtils.ConvertTwoPointsToDirection(monster.Position, path[0]);
            return MoveCore(engine, nextPosition, monster);
        }

        protected bool MoveNearbyOnPath(CoreGameEngine engine, List<Point> path, Monster monster)
        {
            Direction nextPosition = PointDirectionUtils.GetDirectionsNearby(PointDirectionUtils.ConvertTwoPointsToDirection(monster.Position, path[0]))[0];
            return MoveCore(engine, nextPosition, monster);
        }

        protected bool MoveAwayFromPlayer(CoreGameEngine engine, Monster monster)
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

        protected List<ICharacter> OtherNearbyEnemies(CoreGameEngine engine, Monster monster)
        {
            return engine.MonstersInCharactersLOS(monster).Where(x => PointDirectionUtils.NormalDistance(x.Position, engine.Player.Position) < 4).ToList();
        }

        protected bool AreOtherNearbyEnemies(CoreGameEngine engine, Monster monster)
        {
            return OtherNearbyEnemies(engine, monster).Count() > 1;
        }

        protected bool IsPlayerVisible(CoreGameEngine engine, Monster monster)
        {
            return engine.FOVManager.VisibleSingleShot(engine.Map, monster.Position, monster.Vision, engine.Player.Position);
        }

        protected bool WanderRandomly(CoreGameEngine engine, Monster monster)
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
