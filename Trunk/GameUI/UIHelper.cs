
using System.Collections.Generic;
using libtcod;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public static class UIHelper
    {
        // If we allow other resolutions, we'll need to provide a list of 'good'
        // settings here. We always need an odd MapWidth and such, so just doing math is hard.
        public const int ScreenWidth = 80;
        public const int ScreenHeight = 60;
        public const int MapWidth = 53;
        public const int MapHeight = 45;
        public const int CharInfoWidth = 27;
        public const int TextBoxHeight = 15;
        public const int CharInfoHeight = ScreenHeight - TextBoxHeight;

        private const string Font = "arial12x12.png";
        private const int NumberCharsHorz = 32;
        private const int NumberCharsVert = 8;

        public static TCODColor ForegroundColor
        {
            get
            {
                return ColorPresets.LightGray;
            }
        }

        public static TCODConsole SetupUI()
        {
            TCODConsole.setCustomFont(Font, (int)(TCODFontFlags.Grayscale | TCODFontFlags.LayoutTCOD), NumberCharsHorz, NumberCharsVert);
            TCODConsole.initRoot(ScreenWidth, ScreenHeight, "MageCrawl", (bool)Preferences.Instance["Fullscreen"], TCODRendererType.SDL);

            TCODSystem.setFps(30);
            TCODConsole rootConsole = TCODConsole.root;
            rootConsole.setForegroundColor(ForegroundColor);
            rootConsole.setAlignment(TCODAlignment.LeftAlignment);
            rootConsole.setBackgroundFlag(TCODBackgroundFlag.Set);
            return rootConsole;
        }
    }
}
