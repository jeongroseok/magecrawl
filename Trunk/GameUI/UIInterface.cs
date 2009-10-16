
using System.Collections.Generic;
using libtcodWrapper;

namespace GameUI
{
    public static class UIHelper
    {
        public const int ScreenWidth = 80;
        public const int ScreenHeight = 60;
        private const string Font = "arial12x12.png";
        private const int NumberCharsHorz = 32;
        private const int NumberCharsVert = 8;
        private static CustomFontRequestFontTypes flags = CustomFontRequestFontTypes.Grayscale | CustomFontRequestFontTypes.LayoutTCOD;
        private static CustomFontRequest fontReq = new CustomFontRequest(Font, flags, NumberCharsHorz, NumberCharsVert);

        public static RootConsole SetupUI()
        {
            RootConsole.Width = ScreenWidth;
            RootConsole.Height = ScreenHeight;
            RootConsole.Font = fontReq;
            RootConsole.WindowTitle = "MageCrawl";
            RootConsole.Fullscreen = false;
            TCODSystem.FPS = 30;
            return libtcodWrapper.RootConsole.GetInstance();
        }
    }
}
