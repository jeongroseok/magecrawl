
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public static class UIHelper
    {
        // If we allow other resolutions, we'll need to provide a list of 'good'
        // settings here. We always need an odd MapWidth and such, so just doing math is hard.
        public const int ScreenWidth = 80;
        public const int ScreenHeight = 60;
        public const int MapWidth = 51;
        public const int MapHeight = 43;
        public const int CharInfoWidth = 29;
        public const int CharInfoHeight = 60;
        public const int TextBoxHeight = 17;

        private const string Font = "arial12x12.png";
        private const int NumberCharsHorz = 32;
        private const int NumberCharsVert = 8;
        private static CustomFontRequestFontTypes flags = CustomFontRequestFontTypes.Grayscale | CustomFontRequestFontTypes.LayoutTCOD;
        private static CustomFontRequest fontReq = new CustomFontRequest(Font, flags, NumberCharsHorz, NumberCharsVert);

        public static Color ForegroundColor
        {
            get
            {
                return ColorPresets.LightGray;
            }
        }

        public static RootConsole SetupUI()
        {
            RootConsole.Width = ScreenWidth;
            RootConsole.Height = ScreenHeight;
            RootConsole.Font = fontReq;
            RootConsole.WindowTitle = "MageCrawl";
            RootConsole.Fullscreen = (bool)Preferences.Instance["Fullscreen"];
            TCODSystem.FPS = 30;
            RootConsole rootConsole = libtcodWrapper.RootConsole.GetInstance();
            rootConsole.ForegroundColor = ForegroundColor;
            return rootConsole;
        }
    }
}
