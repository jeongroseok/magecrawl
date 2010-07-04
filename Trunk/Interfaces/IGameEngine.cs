using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public delegate void PlayerDied();

    public enum ShowRangedAttackType
    {
        RangedBoltOrBlast, 
        Cone, 
        RangedExplodingPoint 
    }

    // attackingMethod can be an IWeapon, ISpell, IItem
    public delegate void RangedAttack(object attackingMethod, ShowRangedAttackType type, object data, bool targetAtEndPoint);

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

    public enum StairMovmentType
    {
        None, QuitGame, WinGame
    }

    public interface IGameEngine : IDisposable
    {
        void CreateNewWorld(string playerName);        
        void LoadSaveFile(string saveGameName);
        void Save();

        event TextOutputFromGame TextOutputEvent;
        event PlayerDied PlayerDiedEvent;
        event RangedAttack RangedAttackEvent;

        IPlayer Player
        {
            get;
        }

        IMap Map
        {
            get;
        }

        int CurrentLevel
        {
            get;
        }

        int TurnCount 
        {
            get;
        }
        
        IGameState GameState
        {
            get;
        }

        IEngineActions Actions
        {
            get;
        }

        IDebugger Debugger
        {
            get;
        }

        ITargettingUtils Targetting
        {
            get;
        }
    }
}
