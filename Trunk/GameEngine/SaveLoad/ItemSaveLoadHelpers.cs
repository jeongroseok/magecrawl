using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.Weapons;

namespace Magecrawl.GameEngine.SaveLoad
{
    internal static class ItemSaveLoadHelpers
    {
        internal static Item CreateItemObjectFromTypeString(string s)
        {
            switch (s)
            {
                case "Wooden Sword":
                    return new WoodenSword();
                case "Simple Short Bow":
                    return new SimpleShortBow();
                case "Bronze Spear":
                    return new BronzeSpear();
                default:
                    throw new System.ArgumentException("Invalid type in CreateItemObjectFromTypeString");
            }
        }
    }
}
