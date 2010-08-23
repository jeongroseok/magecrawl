using System.Collections.Generic;
using libtcod;
using Magecrawl.Actors;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal class TreasureGenerator : ITreasureGenerator
    {
        public static TreasureGenerator Instance = new TreasureGenerator();

        private TCODRandom s_random = new TCODRandom();

        private TreasureGenerator()
        {
        }

        public void DropTreasureFromMonster(Monster monster)
        {
            // Non-intelligent monsters don't drop anything
            if (!monster.Intelligent)
                return;

            int chance = s_random.getInt(0, 99);
            if (chance < 5)
            {
                DropItem(ItemFactory.Instance.CreateRandomArmor(CoreGameEngine.Instance.CurrentLevel + 1), monster.Position);
                return;
            }
            else if (chance < 10)
            {
                DropItem(ItemFactory.Instance.CreateRandomWeapon(CoreGameEngine.Instance.CurrentLevel + 1), monster.Position);
                return;
            }
            else if (chance < 15)
            {
                DropItem(ItemFactory.Instance.CreateRandomItem(CoreGameEngine.Instance.CurrentLevel + 1), monster.Position);
                return;
            }
        }

        public void GenerateTreasureChestTreasure(ICharacter actor, Point position)
        {
            // At least one item
            GiveItem(actor, ItemFactory.Instance.CreateRandomItem(CoreGameEngine.Instance.CurrentLevel + 1), position);

            // And one armor the player should be able to wear
            List<ArmorWeight> armorWeightsAllowed = new List<ArmorWeight>() { ArmorWeight.Light };
            if (CoreGameEngine.Instance.Player.HasAttribute("StandardArmor"))
                armorWeightsAllowed.Add(ArmorWeight.Standard);
            if (CoreGameEngine.Instance.Player.HasAttribute("HeavyArmor"))
                armorWeightsAllowed.Add(ArmorWeight.Heavy);
            GiveItem(actor, ItemFactory.Instance.CreateRandomArmorOfPossibleWeights(CoreGameEngine.Instance.CurrentLevel + 1, armorWeightsAllowed), position);

            if (s_random.Chance(50))
                GiveItem(actor, ItemFactory.Instance.CreateRandomArmor(CoreGameEngine.Instance.CurrentLevel + 1), position);

            if (s_random.Chance(50))
                GiveItem(actor, ItemFactory.Instance.CreateRandomWeapon(CoreGameEngine.Instance.CurrentLevel + 1), position);

            if (s_random.Chance(50))
                GiveItem(actor, ItemFactory.Instance.CreateRandomItem(CoreGameEngine.Instance.CurrentLevel + 1), position);
        }

        private void DropItem(Item newItem, Point position)
        {
            CoreGameEngine.Instance.Map.AddItem(new Pair<Item, Point>(newItem, position));
        }

        private void GiveItem(ICharacter actor, Item newItem, Point position)
        {
            CoreGameEngine.Instance.SendTextOutput(string.Format("{0} finds a {1}", actor.Name, newItem.DisplayName));
            if (actor is Player)
            {
                ((Player)actor).TakeItem(newItem);
            }
            else
            {
                DropItem(newItem, position);
            }
        }
    }
}
