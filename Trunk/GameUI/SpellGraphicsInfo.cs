using libtcod;
using Magecrawl.GameUI.Utilities;

namespace Magecrawl.GameUI
{
    public static class SpellGraphicsInfo
    {
        public static TCODColor GetColorOfSpellFromSchool(string schoolName)
        {
            switch (schoolName)
            {
                case "Light":
                    return ColorPresets.Wheat;
                case "Darkness":
                    return ColorPresets.DarkGray;
                case "Fire":
                    return ColorPresets.Firebrick;
                case "Arcane":
                    return ColorPresets.DarkViolet;
                case "Air":
                    return ColorPresets.LightBlue;
                case "Earth":
                    return ColorPresets.SaddleBrown;
                case "Water":
                    return ColorPresets.SteelBlue;
                default:
                    return ColorPresets.White;
            }
        }
    }
}
