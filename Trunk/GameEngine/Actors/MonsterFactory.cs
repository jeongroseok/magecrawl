using System;
using System.Collections.Generic;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class MonsterFactory
    {
        public Monster CreateMonster(string name)
        {
            switch (name)
            {
                case "Monster":
                    return new Monster();
                default:
                    throw new System.ArgumentException("Invalid type in MonsterFactory:CreateMonster");
            }
        }
    }
}
