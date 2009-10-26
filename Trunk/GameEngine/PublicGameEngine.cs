using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Magic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    // So in the current archtecture, each public method should do the action requested,
    // and _then_ call the CoreTimingEngine somehow to let others have their time slice before returning
    // This is very synchronous, but easy to do.
    public class PublicGameEngine : IGameEngine
    {
        private CoreGameEngine m_engine;

        public PublicGameEngine(TextOutputFromGame textOutput, PlayerDiedDelegate diedDelegate)
        {
            // This is a singleton accessable from anyone in GameEngine, but stash a copy since we use it alot
            m_engine = new CoreGameEngine(textOutput, diedDelegate);
        }

        public void Dispose()
        {
            if (m_engine != null)
                m_engine.Dispose();
            m_engine = null;
        }

        public Point TargetSelection
        {
            get;
            set;
        }

        public bool SelectingTarget
        {
            get;
            set;
        }

        public IPlayer Player
        {
            get
            {
                return m_engine.Player;
            }
        }

        public IMap Map
        {
            get
            {
                return m_engine.Map;
            }
        }

        public bool MovePlayer(Direction direction)
        {
            bool didAnything = m_engine.Move(m_engine.Player, direction);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool Operate(Direction direction)
        {
            bool didAnything = m_engine.Operate(m_engine.Player, direction);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool PlayerWait()
        {
            bool didAnything = m_engine.Wait(m_engine.Player);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }

        public bool PlayerAttack(Point target)
        {
            bool didAnything = m_engine.Attack(m_engine.Player, target);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;            
        }

        public bool PlayerCastSpell(string spellName)
        {
            SpellBase spell = SpellFactory.CreateSpell(spellName);
            if (m_engine.Player.CurrentMagic >= spell.MagicCost)
            {
                m_engine.CastSpell(m_engine.Player, spell);
                m_engine.Player.CurrentMagic -= spell.MagicCost;
                m_engine.AfterPlayerAction();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool PlayerGetItem()
        {
            bool didAnything = m_engine.PlayerGetItem();
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;            
        }

        public void Save()
        {
            m_engine.Save();
        }

        public void Load()
        {
            m_engine.Load();
        }

        public IList<Magecrawl.Utilities.Point> PlayerPathToPoint(Magecrawl.Utilities.Point dest)
        {
            return m_engine.PathToPoint(m_engine.Player, dest, true);
        }

        // For the IsPathable debugging mode, show if player could walk there.
        public bool[,] PlayerMoveableToEveryPoint()
        {
            return m_engine.PlayerMoveableToEveryPoint();
        }

        public List<Point> CellsInPlayersFOV()
        {
            return GenerateFOVListForCharacter(m_engine.Player);
        }

        private List<Point> GenerateFOVListForCharacter(ICharacter c)
        {
            List<Point> returnList = new List<Point>();

            m_engine.FOVManager.CalculateForMultipleCalls(c.Position, c.Vision);

            for (int i = 0; i < m_engine.Map.Width; ++i)
            {
                for (int j = 0; j < m_engine.Map.Height; ++j)
                {
                    Point currentPosition = new Point(i, j);
                    if (m_engine.FOVManager.Visible(currentPosition))
                    {
                        returnList.Add(currentPosition);
                    }
                }
            }
            return returnList;
        }

        public Dictionary<ICharacter, List<Point>> CellsInAllMonstersFOV()
        {
            Dictionary<ICharacter, List<Point>> returnValue = new Dictionary<ICharacter, List<Point>>();

            foreach (ICharacter c in m_engine.Map.Monsters)
            {
                returnValue[c] = GenerateFOVListForCharacter(c);
            }

            return returnValue;
        }

        public TileVisibility[,] CalculateTileVisibility()
        {
            return m_engine.CalculateTileVisibility();
        }

        public List<ItemOptions> GetOptionsForInventoryItem(IItem item)
        {
            return m_engine.GetOptionsForInventoryItem(item);
        }

        public bool PlayerSelectedItemOption(IItem item, string option)
        {
            bool didAnything = m_engine.PlayerSelectedItemOption(item, option);
            if (didAnything)
                m_engine.AfterPlayerAction();
            return didAnything;
        }
    }
}
