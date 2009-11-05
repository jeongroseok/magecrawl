using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class MonsterFactory
    {
        public Monster CreateMonster(string name)
        {
            return CreateMonster(name, Point.Invalid);
        }

        public Monster CreateMonster(string name, Point p)
        {
            switch (name)
            {
                case "Monster":
                    return new Monster(p);
                default:
                    throw new System.ArgumentException("Invalid type in MonsterFactory:CreateMonster");
            }
        }
    }
}
