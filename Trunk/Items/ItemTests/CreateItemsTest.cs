using System.Collections.Generic;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using NUnit.Framework;

namespace Magecraw.Items.Tests
{
    [TestFixture]
    public class CreateItemsTests
    {
        private const int IterationsOfEachLevel = 5000;
        private const int MaxItemLevel = 5;

        [Test]
        public void CreateMeleeWeapon()
        {
            IWeapon item = ItemFactory.Instance.CreateMeleeWeapon(null);
            Assert.IsNotNull(item);
        }

        [Test]
        public void CreateBaseItemOfEveryType()
        {
            foreach (string type in ItemFactory.Instance.ItemTypeList)
            {
                Item item = ItemFactory.Instance.CreateBaseItem(type);
                Assert.IsNotNull(item);
            }
        }

        [Test]
        public void CreateRandomItemsOfEachLevel()
        {
            RunForEachLevelIteration(level =>
            {
                Item item = ItemFactory.Instance.CreateRandomItem(level);
                Assert.IsNotNull(item);
            });  
        }

        [Test]
        public void CreateRandomWeaponOfEachLevel()
        {
            RunForEachLevelIteration(level =>
            {
                Item item = ItemFactory.Instance.CreateRandomWeapon(level);
                Assert.IsNotNull(item);
            });            
        }

        [Test]
        public void CreateRandomArmorOfEachLevel()
        {
            RunForEachLevelIteration(level =>
            {
                Item item = ItemFactory.Instance.CreateRandomArmor(level);
                Assert.IsNotNull(item);
            });  
        }

        [Test]
        public void CreateRandomArmorOfEachLevelForEachWeight()
        {
            RunForEachLevelIteration(level =>
            {
                Item item = ItemFactory.Instance.CreateRandomArmorOfPossibleWeights(level, new List<ArmorWeight>() { ArmorWeight.Light });
                Assert.IsNotNull(item);
            });

            RunForEachLevelIteration(level =>
            {
                Item item = ItemFactory.Instance.CreateRandomArmorOfPossibleWeights(level, new List<ArmorWeight>() { ArmorWeight.Standard });
                Assert.IsNotNull(item);
            });

            RunForEachLevelIteration(level =>
            {
                Item item = ItemFactory.Instance.CreateRandomArmorOfPossibleWeights(level, new List<ArmorWeight>() { ArmorWeight.Heavy });
                Assert.IsNotNull(item);
            });
        }

        private delegate void RunEachIteartionCallback(int level);
        private void RunForEachLevelIteration(RunEachIteartionCallback runCallback)
        {
            for (int level = 0; level <= 5; ++level)
            {
                for (int i = 0; i < IterationsOfEachLevel; ++i)
                {
                    runCallback(level);
                }
            }
        }
    }
}
