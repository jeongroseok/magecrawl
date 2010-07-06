using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public interface IGameState
    {
        bool DangerInLOS();
        bool CurrentOrRecentDanger();

        List<ICharacter> MonstersInPlayerLOS();
        
        // If you go up on level 0 or down at end, dialog should come up to let them know what's going on
        StairMovmentType IsStairMovementSpecial(bool headingUp);

        List<ItemOptions> GetOptionsForInventoryItem(IItem item);
        List<ItemOptions> GetOptionsForEquipmentItem(IItem item);

        List<string> GetDescriptionForTile(Point p);
        
        TileVisibility[,] CalculateTileVisibility();
        List<EffectivePoint> CalculateTargetablePointsForEquippedWeapon();

        ISkill GetSkillFromName(string name);
    }
}
