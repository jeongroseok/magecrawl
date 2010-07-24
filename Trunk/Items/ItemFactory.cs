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

        private static ItemFactory m_instance = null;
        public static ItemFactory Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ItemFactory();
                return m_instance;
            }
        }

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
                case "Club":
                case "Dagger":                    
                case "Spear":
                case "Sword":
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

        private static List<string> s_itemList = new List<string>() { "Axe", "Bow", "Club", "Dagger", "Sling", "Spear", 
            "Sword", "ChestArmor", "Helm", "Gloves", "Boots", "Potion", "Scroll", "Wand" };
        public List<string> ItemTypeList
        {
            get
            {
                return s_itemList;
            }
        }

        public Item CreateRandomItem(int level)
        {
            return CreateItemOfType(ItemTypeList.Randomize()[0], level);
        }

        public Item CreateItemOfType(string type, int level)
        {
            return CreateItemOfType(type, level, null);
        }

        public Item CreateItemOfType(string type, int level, string qualityName)
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
                returnItem = CreateItemOfTypeCore(type, level, lowLevel, highLevel, qualityName);
                if (returnItem != null)
                    return returnItem;
                i++;
            }
        }

        public Item CreateItemOfTypeCore(string type, int level, int lowLevel, int highLevel, string qualityName)
        {            
            switch (type)
            {
                case "Axe":
                case "Club":
                case "Dagger":                    
                case "Spear":
                case "Sword":
                {
                    Material material;
                    Quality quality;
                    CalculateMaterials(type, level, lowLevel, highLevel, qualityName, out material, out quality);
                    if (material == null || quality == null)
                        return null;

                    return new StatsBasedWeapon(WeaponRangeFactory.Create(type), material, quality);
                }
                case "Sling":
                case "Bow":
                {
                    Material material;
                    Quality quality;
                    CalculateMaterials(type, level, lowLevel, highLevel, qualityName, out material, out quality);
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
                    CalculateMaterials(type, level, lowLevel, highLevel, qualityName, out material, out quality);
                    if (material == null || quality == null)
                        return null;

                    return new Armor(type, material, quality);
                }
                case "Potion":
                case "Scroll":
                {
                    ConsumableEffect effect = ConsumableEffectFactory.GetMaterialInLevelRange(type, lowLevel, highLevel);
                    if (effect == null)
                        return null;

                    int levelCapLeft = (int)Math.Max(highLevel - effect.ItemLevel, 0);
                        
                    // For every two level we go below the cap, increase caster level by 1
                    effect.CasterLevel += levelCapLeft / 2;

                    return new Consumable(type, effect, 1, 1);
                }
                case "Wand":
                {
                    // Wands reduce the level of the item in half since they have charges
                    ConsumableEffect effect = ConsumableEffectFactory.GetMaterialInLevelRange(type, lowLevel / 2, highLevel / 2);
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

        private void CalculateMaterials(string type, int level, int lowLevel, int highLevel, string qualityName, out Material material, out Quality quality)
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

            int minLevelCapNeeded = (int)Math.Max(lowLevel - quality.LevelAdjustment, 0);
            int levelCapLeft = (int)Math.Max(highLevel - quality.LevelAdjustment, 0);

            material = MaterialFactory.GetMaterialInLevelRange(type, minLevelCapNeeded, levelCapLeft);
        }
    }
}
