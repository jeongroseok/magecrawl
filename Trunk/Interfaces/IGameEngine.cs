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
        
        void Save();

        bool CouldCastSpell(ISpell spell);
        
        // If you go up on level 0 or down at end, dialog should come up to let them know what's going on
        StairMovmentType IsStairMovementSpecial(bool headingUp);

        List<Point> PlayerPathToPoint(Point dest);
        bool DangerInLOS();
        bool CurrentOrRecentDanger();
        List<ICharacter> MonstersInPlayerLOS();

        // Takes either an IItem or ISpell
        List<Point> TargettedDrawablePoints(object targettingObject, Point target);

        bool IsRangedPathBetweenPoints(Point x, Point y);
        void FilterNotVisibleBothWaysFromList(List<EffectivePoint> pointList, bool savePlayerPositionFromList);

        List<ItemOptions> GetOptionsForInventoryItem(IItem item);
        List<ItemOptions> GetOptionsForEquipmentItem(IItem item);
        TargetingInfo GetTargettingTypeForInventoryItem(IItem item, string action);

        TileVisibility[,] CalculateTileVisibility();
        void FilterNotTargetablePointsFromList(List<EffectivePoint> pointList, bool needsToBeVisible);

        List<string> GetDescriptionForTile(Point p);
        
        ISkill GetSkillFromName(string name);

        IEngineActions Actions
        {
            get;
        }

        IDebugger Debugger
        {
            get;
        }
    }
}
