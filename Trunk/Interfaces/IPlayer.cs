using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public enum EquipArmorReasons { None, Weight, RobesPreventBoots, BootsPreventRobes }

    public interface IPlayer : ICharacter
    {
        int CurrentStamina 
        {
            get;
        }

        int CurrentHealth
        {
            get;
        }

        int MaxStamina
        {
            get;
        }

        int MaxHealth
        {
            get;
        }

        int CurrentMP
        {
            get;
        }

        int MaxMP
        {
            get;
        }

        int MaxPossibleMP
        {
            get;
        }

        int SkillPoints
        {
            get;
        }

        IEnumerable<IItem> Items
        {
            get;
        }

        IEnumerable<ISpell> Spells
        {
            get;
        }

        IEnumerable<ISkill> Skills
        {
            get;
        }

        IEnumerable<IStatusEffect> StatusEffects
        {
            get;
        }

        IWeapon SecondaryWeapon
        {
            get;
        }

        IArmor ChestArmor
        {
            get;
        }

        IArmor Headpiece
        {
            get;
        }
        
        IArmor Gloves
        {
            get;
        }

        IArmor Boots
        {
            get;
        }

        bool CouldCastSpell(ISpell spell);

        bool CanEquipArmor(IArmor armor);
        IList<EquipArmorReasons> CanNotEquipArmorReasons(IArmor armor);
    }
}
