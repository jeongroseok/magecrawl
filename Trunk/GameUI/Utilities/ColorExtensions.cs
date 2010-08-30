using System;
using libtcod;

namespace Magecrawl.GameUI.Utilities
{
    public static class ColorExtensions
    {
        public static TCODColor Divide(this TCODColor c, double amt)
        {
            return new TCODColor((int)(c.Red / amt), (int)(c.Green / amt), (int)(c.Blue / amt));
        }
    }
}
