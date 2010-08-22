using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Skills;
using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine
{
    internal static class PlayerBackgrounds
    {
        internal static void SetupBackground(Player player, string startingBackground)
        {
            switch (startingBackground)
            {
                case "Scholar":
                {
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Basic Staff Proficiency"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Blessing Of Mana I"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Blessing Of Mana II"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Staff", 0, "Wood", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("ChestArmor", 0, "Linen Cloth", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Helm", 0, "Linen Cloth", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Gloves", 0, "Linen Cloth", "Average"));
                    player.TakeItem(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Potion", 0));
                    player.TakeItem(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Potion", 0));
                    player.TakeItem(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Potion", 1));
                    player.TakeItem(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Scroll", 1));
                    player.SkillPoints = 15;
                    break;
                }
                case "Scout":
                {
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Basic Dagger Proficiency"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Basic Sling Proficiency"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Armor Proficiency"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Toughness I"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Light"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Dagger", 0, "Wrought Iron", "Average"));
                    player.EquipSecondaryWeapon((IWeapon)CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Sling", 0, "Jute", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("ChestArmor", 0, "Light Leather", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Helm", 0, "Light Leather", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Gloves", 0, "Light Leather", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Boots", 0, "Light Leather", "Average"));
                    player.TakeItem(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Potion", 0));
                    player.TakeItem(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Scroll", 1));
                    player.SkillPoints = 0;
                    break;
                }
                case "Templar":
                {
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Basic Sword Proficiency"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Armor Proficiency"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Heavy Armor Proficiency"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Sword", 0, "Bronze", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("ChestArmor", 0, "Bronze", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Helm", 0, "Bronze", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Gloves", 0, "Bronze", "Average"));
                    player.Equip(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Boots", 0, "Bronze", "Average"));
                    player.TakeItem(CoreGameEngine.Instance.ItemFactory.CreateItemOfType("Potion", 0));
                    player.SkillPoints = 0;
                    break;
                }
                default:
                    throw new InvalidOperationException("SetupBackground with invalid background - " + startingBackground);
            }

            player.SetHPMPMax();
        }
    }
}
