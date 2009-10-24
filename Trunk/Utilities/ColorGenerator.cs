using System;
using System.Collections.Generic;
using libtcodWrapper;

namespace Magecrawl.Utilities
{
    public static class ColorGenerator
    {
        private static TCODRandom s_randomColorRNG = new TCODRandom();
        public static Color GenerateRandomColor(bool pastels)
        {
            int red = s_randomColorRNG.GetRandomInt(0, 255);
            int green = s_randomColorRNG.GetRandomInt(0, 255);
            int blue = s_randomColorRNG.GetRandomInt(0, 255);

            // If you want pastels
            if (pastels)
            {
                red = (red + TCODColorPresets.White.Red) / 2;
                green = (green + TCODColorPresets.White.Green) / 2;
                blue = (blue + TCODColorPresets.White.Blue) / 2;
            }

            return Color.FromRGB((byte)red, (byte)green, (byte)blue);
        }
    }
}
