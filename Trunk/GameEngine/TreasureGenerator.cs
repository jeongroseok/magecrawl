using System.Collections.Generic;
using libtcod;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal static class TreasureGenerator
    {
        private static TCODRandom s_random = new TCODRandom();

        internal static void DropTreasureFromMonster(Monster monster)
        {
            // Non-intelligent monsters don't drop anything
            if (!monster.Intelligent)
                return;

            int chance = s_random.getInt(0, 99);
            if (chance < 5)
            {
                DropItem(CoreGameEngine.Instance.ItemFactory.CreateRandomArmor(CoreGameEngine.Instance.CurrentLevel + 1), monster.Position);
                return;
            }
            else if (chance < 10)
            {
                DropItem(CoreGameEngine.Instance.ItemFactory.CreateRandomWeapon(CoreGameEngine.Instance.CurrentLevel + 1), monster.Position);
                return;
            }
            else if (chance < 15)
            {
                DropItem(CoreGameEngine.Instance.ItemFactory.CreateRandomItem(CoreGameEngine.Instance.CurrentLevel + 1), monster.Position);
                return;
            }
        }

        internal static void GenerateTreasureChestTreasure(ICharacter actor, Point position)
        {
            // At least one item
            GiveItem(actor, CoreGameEngine.Instance.ItemFactory.CreateRandomItem(CoreGameEngine.Instance.CurrentLevel + 1), position);

            // And one armor the player should be able to wear
            List<ArmorWeight> armorWeightsAllowed = new List<ArmorWeight>() { ArmorWeight.Light };
            if (CoreGameEngine.Instance.Player.HasAttribute("StandardArmor"))
                armorWeightsAllowed.Add(ArmorWeight.Standard);
            if (CoreGameEngine.Instance.Player.HasAttribute("HeavyArmor"))
                armorWeightsAllowed.Add(ArmorWeight.Heavy);
            GiveItem(actor, CoreGameEngine.Instance.ItemFactory.CreateRandomArmorOfPossibleWeights(CoreGameEngine.Instance.CurrentLevel + 1, armorWeightsAllowed), position);

            if (s_random.Chance(50))
                GiveItem(actor, CoreGameEngine.Instance.ItemFactory.CreateRandomArmor(CoreGameEngine.Instance.CurrentLevel + 1), position);

            if (s_random.Chance(50))
                GiveItem(actor, CoreGameEngine.Instance.ItemFactory.CreateRandomWeapon(CoreGameEngine.Instance.CurrentLevel + 1), position);

            if (s_random.Chance(50))
                GiveItem(actor, CoreGameEngine.Instance.ItemFactory.CreateRandomItem(CoreGameEngine.Instance.CurrentLevel + 1), position);
        }

        private static void DropItem(Item newItem, Point position)
        {
            CoreGameEngine.Instance.Map.AddItem(new Pair<Item, Point>(newItem, position));
        }

        private static void GiveItem(ICharacter actor, Item newItem, Point position)
        {
            CoreGameEngine.Instance.SendTextOutput(string.Format("{0} finds a {1}", actor.Name, newItem.DisplayName));
            if (actor is Player)
            {
                ((Player)actor).TakeItem(newItem);
            }
            else
            {
                DropItem(newItem, position);
#if DEBUG
                throw new System.NotImplementedException("Non players opening chests?");
#endif
            }
        }
    }
}
