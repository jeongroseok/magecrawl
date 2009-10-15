
using System.Collections.Generic;
using libtcodWrapper;

namespace GameUI
{
    public static class UIHelper
    {
        private static string font = "arial12x12.png";
        private static int numberCharsHorz = 32;
        private static int numberCharsVert = 8;
        private static CustomFontRequestFontTypes flags = CustomFontRequestFontTypes.Grayscale | CustomFontRequestFontTypes.LayoutTCOD;
        private static CustomFontRequest fontReq = new CustomFontRequest(font, flags, numberCharsHorz, numberCharsVert);

        public static RootConsole SetupUI()
        {
            RootConsole.Width = 80;
            RootConsole.Height = 60;
            RootConsole.Font = fontReq;
            RootConsole.WindowTitle = "MageCrawl";
            RootConsole.Fullscreen = false;
            TCODSystem.FPS = 30;
            return libtcodWrapper.RootConsole.GetInstance();
        }
    }
}
