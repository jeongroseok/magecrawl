﻿using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public delegate void PlayerDiedDelegate();
    public delegate void TextOutputFromGame(string s);

    public enum TileVisibility 
    {
        Unvisited, Visited, Visible 
    }

    public struct ItemOptions
    {
        public ItemOptions(string option, bool enabled)
        {
            Option = option;
            Enabled = enabled;
        }
        public string Option;
        public bool Enabled;
    }

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
        bool PlayerGetItem();
        bool PlayerAttack(Point target);
        bool PlayerCastSpell(string spellName);
        IList<Point> PlayerPathToPoint(Point dest);
        List<Point> CellsInPlayersFOV();

        List<ItemOptions> GetOptionsForInventoryItem(IItem item);
        // TODO: What to do here when you zap a want and need a target?
        bool PlayerSelectedItemOption(IItem item, string option);

        void IterateThroughWeapons();

        TileVisibility[,] CalculateTileVisibility();
        
        // Debugging calls
        bool[,] PlayerMoveableToEveryPoint();
        Dictionary<ICharacter, List<Point>> CellsInAllMonstersFOV();
    }
}
