using System;
using System.Collections.Generic;
using libtcod;

namespace Magecrawl.Utilities
{
    public static class ColorGenerator
    {
        private static Random s_randomColorRNG = new Random();
        public static TCODColor GenerateRandomColor(bool pastels)
        {
            int red = s_randomColorRNG.getInt(0, 255);
            int green = s_randomColorRNG.getInt(0, 255);
            int blue = s_randomColorRNG.getInt(0, 255);

            // If you want pastels
            if (pastels)
            {
                red = (red + TCODColor.white.Red) / 2;
                green = (green + TCODColor.white.Green) / 2;
                blue = (blue + TCODColor.white.Blue) / 2;
            }

            return new TCODColor((byte)red, (byte)green, (byte)blue);
        }
    }
}
