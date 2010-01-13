using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public delegate void PlayerDiedDelegate();
    public delegate void RangedAttackAgainstPlayer(List<Point> rangedPath);
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

        bool MovePlayer(Direction direction);
        bool Operate(Point pointToOperateAt);
        bool PlayerWait();
        void Save();
        bool PlayerGetItem();
        bool PlayerAttack(Point target);
        bool PlayerCouldCastSpell(ISpell spell);
        bool PlayerCastSpell(ISpell spell, Point target);
        bool ReloadWeapon();
        
        // If you go up on level 0 or down at end, dialog should come up to let them know what's going on
        StairMovmentType IsStairMovementSpecial(bool headingUp);

        bool PlayerMoveDownStairs();
        bool PlayerMoveUpStairs();

        List<Point> PlayerPathToPoint(Point dest);
        List<Point> CellsInPlayersFOV();
        bool DangerInLOS();
        List<ICharacter> MonstersInPlayerLOS();

        bool PlayerSwapPrimarySecondaryWeapons();
        List<ItemOptions> GetOptionsForInventoryItem(IItem item);
        List<ItemOptions> GetOptionsForEquipmentItem(IItem item);
        
        // TODO: What to do here when you zap a want and need a target?
        bool PlayerSelectedItemOption(IItem item, string option);

        TileVisibility[,] CalculateTileVisibility();
        void FilterNotTargetablePointsFromList(List<EffectivePoint> pointList, bool needsToBeVisible);
        
        // Debugging calls
        bool[,] PlayerMoveableToEveryPoint();
        Dictionary<ICharacter, List<Point>> CellsInAllMonstersFOV();
    }
}
