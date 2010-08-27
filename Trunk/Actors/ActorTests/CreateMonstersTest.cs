using Magecrawl.Actors;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;
using NUnit.Framework;

namespace Magecraw.Actors.Tests
{
    [TestFixture]
    public class CreateMonstersTest
    {
        private const int MaxMonsterLevel = 5;
        [Test]
        public void CreateLotsOfMonstersForEachLevel()
        {
            const int IterationsOfEachLevel = 10000;
            Point position = new Point(0, 0);
            for (int level = 0; level <= MaxMonsterLevel; ++level)
            {
                for (int i = 0; i < IterationsOfEachLevel; ++i)
                {
                    Monster newMonster = MonsterFactory.Instance.CreateRandomMonster(level, position);
                    Assert.IsNotNull(newMonster);
                }
            }
        }

        [Test]
        public void CreateMonsterOfEachTypeOfEachLevel()
        {
            for (int level = 0; level <= 5; ++level)
            {
                foreach(INamedItem monsterName in MonsterFactory.Instance.GetAllMonsterListForDebug())
                {
                    try
                    {
                        Monster newMonster = MonsterFactory.Instance.CreateMonster(monsterName.DisplayName, level);
                        Assert.IsNotNull(newMonster);
                    }
                    catch (System.InvalidOperationException)
                    {
                    }
                }
            }
        }

        [Test]
        public void CreateMonsterOfEachTypeOfEachLevelWithPoint()
        {
            for (int level = 0; level <= 5; ++level)
            {
                foreach (INamedItem monsterName in MonsterFactory.Instance.GetAllMonsterListForDebug())
                {
                    try
                    {
                        Monster newMonster = MonsterFactory.Instance.CreateMonster(monsterName.DisplayName, level, new Point(5, 5));
                        Assert.IsNotNull(newMonster);
                    }
                    catch (System.InvalidOperationException)
                    {
                    }
                }
            }
        }
    }
}
