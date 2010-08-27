using System;
using System.Collections.Generic;
using libtcod;
using Magecrawl.Interfaces;
using Magecrawl.Items.Materials;
using Magecrawl.Items.WeaponRanges;
using Magecrawl.Utilities;

namespace Magecrawl.Items
{
    public class ItemFactory
    {
        private static TCODRandom s_random = new TCODRandom();

        internal MaterialFactory MaterialFactory;
        internal QualityFactory QualityFactory;
        internal ComsumableEffectFactory ConsumableEffectFactory;

        public static readonly ItemFactory Instance = new ItemFactory();

        private ItemFactory()
        {
            MaterialFactory = new MaterialFactory();
            QualityFactory = new QualityFactory();
            ConsumableEffectFactory = new ComsumableEffectFactory();
        }

        // The Melee Weapon is somewhat unique as it reflects back the attributes of the wielder
        public IWeapon CreateMeleeWeapon(ICharacter wielder)
        {
            return new MeleeWeapon(wielder);
        }

        public Item CreateBaseItem(string type)
        {
            switch (type)
            {
                case "Axe":
                case "Dagger":                    
                case "Spear":
                case "Sword":
                case "Staff":
                    return new StatsBasedWeapon();
                case "Sling":
                case "Bow":
                    return new StatsBasedRangedWeapon();
                case "MeleeWeapon":
                    return new MeleeWeapon();
                case "ChestArmor":
                case "Helm":
                case "Gloves":
                case "Boots":
                    return new Armor(type);
                case "Potion":
                case "Scroll":
                case "Wand":
                    return new Consumable(type);
                default:
                    throw new System.NotImplementedException("CreateBaseItem of unknown type: " + type);
            }
        }

        private static List<string> s_weaponList = new List<string>() { "Axe", "Bow", "Dagger", "Sling", "Spear", "Staff", "Sword"};
        private static List<string> s_armorList = new List<string>() { "ChestArmor", "Helm", "Gloves", "Boots" };
        private static List<string> s_itemList = new List<string>() { "Potion", "Scroll", "Wand" };
        
        public List<string> ItemTypeList
        {
            get
            {
                List<string> returnList = new List<string>();
                returnList.AddRange(s_weaponList);
                returnList.AddRange(s_armorList);
                returnList.AddRange(s_itemList);
                return returnList;
            }
        }

        public Item CreateRandomItem(int level)
        {
            return CreateItemOfType(s_itemList.Randomize()[0], level);
        }

        public Item CreateRandomWeapon(int level)
        {
            return CreateItemOfType(s_weaponList.Randomize()[0], level);
        }

        public Item CreateRandomArmor(int level)
        {
            return CreateItemOfType(s_armorList.Randomize()[0], level);
        }

        public Item CreateRandomArmorOfPossibleWeights(int level, List<ArmorWeight> weightsAllowed)
        {
            while (true)
            {
                Armor armor = (Armor)CreateItemOfType(s_armorList.Randomize()[0], level);
                if (weightsAllowed.Contains(armor.Weight))
                    return armor;
            }
        }

        public Item CreateItemOfType(string type, int level)
        {
            return CreateItemOfType(type, level, null, null);
        }

        public Item CreateItemByName(string type, string name)
        {
            return CreateItemOfType(type, 0, name, null);
        }

        public Item CreateItemOfType(string type, int level, string materialName, string qualityName)
        {
            // So sometimes there can be "gaps" where we don't have a base material of
            // the preferred 3/4 required. This loop will try again a good number of times, every fifth time 
            // decreasing the required quality until we can generate something.
            Item returnItem = null;
            int i = 0;
            while (true)
            {
                // Lower is 3/4 of requested. High is level.
                // If we fail to produce on first try, reduce low level by 1 every fifth iteration
                int lowLevelReduction = i / 5;
                int lowLevel = Math.Max((int)Math.Round((level * 3.0) / 4.0) - lowLevelReduction, 0);
                int highLevel = level;
                returnItem = CreateItemOfTypeCore(type, level, lowLevel, highLevel, materialName, qualityName);
                if (returnItem != null)
                    return returnItem;
                i++;
            }
        }

        private Item CreateItemOfTypeCore(string type, int level, int lowLevel, int highLevel, string materialName, string qualityName)
        {            
            switch (type)
            {
                case "Axe":
                case "Dagger":                    
                case "Spear":
                case "Staff":
                case "Sword":
                {
                    Material material;
                    Quality quality;
                    GetWeaponArmorParts(type, level, lowLevel, highLevel, materialName, qualityName, out material, out quality);
                    if (material == null || quality == null)
                        return null;

                    return new StatsBasedWeapon(WeaponRangeFactory.Create(type), material, quality);
                }
                case "Sling":
                case "Bow":
                {
                    Material material;
                    Quality quality;
                    GetWeaponArmorParts(type, level, lowLevel, highLevel, materialName, qualityName, out material, out quality);
                    if (material == null || quality == null)
                        return null;

                    return new StatsBasedRangedWeapon(WeaponRangeFactory.Create(type), material, quality);
                }
                case "ChestArmor":
                case "Helm":
                case "Gloves":
                case "Boots":
                {
                    Material material;
                    Quality quality;
                    GetWeaponArmorParts(type, level, lowLevel, highLevel, materialName, qualityName, out material, out quality);
                    if (material == null || quality == null)
                        return null;

                    return new Armor(type, material, quality);
                }
                case "Potion":
                case "Scroll":
                {
                    ConsumableEffect effect;
                    if (materialName != null)
                        effect = ConsumableEffectFactory.GetEffect(type, materialName);
                    else
                        effect = ConsumableEffectFactory.GetEffectInLevelRange(type, lowLevel, highLevel);

                    if (effect == null)
                        return null;

                    int levelCapLeft = (int)Math.Max(highLevel - effect.ItemLevel, 0);

                    return new Consumable(type, effect, 1, 1);
                }
                case "Wand":
                {
                    // Wands reduce the level of the item by 1 since it has charges
                    ConsumableEffect effect;
                    if (materialName != null)
                        effect = ConsumableEffectFactory.GetEffect(type, materialName);
                    else
                        effect = ConsumableEffectFactory.GetEffectInLevelRange(type, Math.Max(0, lowLevel - 1), Math.Max(0, highLevel - 1));

                    if (effect == null)
                        return null;

                    // Start with three charges max
                    int maxCharges = 3;
                    int levelCapLeft = (int)Math.Max(highLevel - effect.ItemLevel, 0);

                    // For every level we go below the cap, increase possible charges by 1
                    maxCharges += levelCapLeft;

                    // But max them at 10
                    maxCharges = Math.Min(maxCharges, 10);

                    int startingCharges = s_random.getInt((maxCharges * 2) / 3, maxCharges);

                    return new Consumable(type, effect, startingCharges, maxCharges);
                }
                default:
                    throw new InvalidOperationException("CreateItemOfType of unknown type - " + type);
            }
        }

        private void GetWeaponArmorParts(string type, int level, int lowLevel, int highLevel, string materialName, string qualityName, out Material material, out Quality quality)
        {
            if (qualityName != null)
                quality = QualityFactory.GetQuality(qualityName);
            else
                quality = QualityFactory.GetQualityNoHigherThan(highLevel);

            if (quality == null)
            {
                material = null;
                return;
            }

            if (materialName != null)
            {
                material = MaterialFactory.GetMaterial(type, materialName);
            }
            else
            {
                int minLevelCapNeeded = (int)Math.Max(lowLevel - quality.LevelAdjustment, 0);
                int levelCapLeft = (int)Math.Max(highLevel - quality.LevelAdjustment, 0);

                material = MaterialFactory.GetMaterialInLevelRange(type, minLevelCapNeeded, levelCapLeft);
            }
        }
    }
}
