using System;
using System.Collections.Generic;
using System.Text;

namespace Magecrawl.Utilities
{
    public struct DiceRoll
    {
        private static Random random = new Random();

        public short Rolls;
        public short DiceFaces;
        public short ToAdd;
        public short Multiplier;

        public DiceRoll(short rolls, short diceFaces, short toAdd)
        {
            Rolls = rolls;
            DiceFaces = diceFaces;
            ToAdd = toAdd;
            Multiplier = 1;
        }

        public DiceRoll(short rolls, short diceFaces, short toAdd, short multiplier)
        {
            Rolls = rolls;
            DiceFaces = diceFaces;
            ToAdd = toAdd;
            Multiplier = multiplier;
        }

        public short Roll()
        {
            short total = 0;
            for (short i = 0; i < Rolls; i++)
            {
                total += (short)(random.Next(DiceFaces) + 1);
            }
            return (short)(Multiplier * (total + ToAdd));
        }
    }
}
