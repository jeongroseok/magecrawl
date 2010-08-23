using System;
using Magecrawl.GameEngine.Skills;
using Magecrawl.Interfaces;
using Magecrawl.Items;

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
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Staff", 0, "Wood", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("ChestArmor", 0, "Linen Cloth", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Helm", 0, "Linen Cloth", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Gloves", 0, "Linen Cloth", "Average"));
                    player.TakeItem(ItemFactory.Instance.CreateItemOfType("Potion", 0));
                    player.TakeItem(ItemFactory.Instance.CreateItemOfType("Potion", 0));
                    player.TakeItem(ItemFactory.Instance.CreateItemOfType("Potion", 1));
                    player.TakeItem(ItemFactory.Instance.CreateItemOfType("Scroll", 1));
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
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Dagger", 0, "Wrought Iron", "Average"));
                    player.EquipSecondaryWeapon((IWeapon)ItemFactory.Instance.CreateItemOfType("Sling", 0, "Jute", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("ChestArmor", 0, "Light Leather", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Helm", 0, "Light Leather", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Gloves", 0, "Light Leather", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Boots", 0, "Light Leather", "Average"));
                    player.TakeItem(ItemFactory.Instance.CreateItemOfType("Potion", 0));
                    player.TakeItem(ItemFactory.Instance.CreateItemOfType("Scroll", 1));
                    player.SkillPoints = 0;
                    break;
                }
                case "Templar":
                {
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Basic Sword Proficiency"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Armor Proficiency"));
                    player.AddSkill(SkillFactory.Instance.CreateSkill("Heavy Armor Proficiency"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Sword", 0, "Bronze", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("ChestArmor", 0, "Bronze", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Helm", 0, "Bronze", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Gloves", 0, "Bronze", "Average"));
                    player.Equip(ItemFactory.Instance.CreateItemOfType("Boots", 0, "Bronze", "Average"));
                    player.TakeItem(ItemFactory.Instance.CreateItemOfType("Potion", 0));
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
