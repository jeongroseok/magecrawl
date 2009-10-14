using System;
using System.Collections.Generic;

namespace MageCrawl
{
    public static class EntryPoint
    {
        public static void Main()
        {
            string font = "celtic_garamond_10x10_gs_tc.png";
            int numberCharsHorz = 32;
            int numberCharsVert = 8;
            libtcodWrapper.CustomFontRequestFontTypes flags = libtcodWrapper.CustomFontRequestFontTypes.Grayscale | libtcodWrapper.CustomFontRequestFontTypes.LayoutTCOD;

            libtcodWrapper.CustomFontRequest fontReq = new libtcodWrapper.CustomFontRequest(font, flags, numberCharsHorz, numberCharsVert);

            libtcodWrapper.RootConsole.Width = 80;
            libtcodWrapper.RootConsole.Height = 50;
            libtcodWrapper.RootConsole.Font = fontReq;
            libtcodWrapper.RootConsole.WindowTitle = "MageCrawl";
            libtcodWrapper.RootConsole.Fullscreen = false;
            libtcodWrapper.RootConsole rootConsole = libtcodWrapper.RootConsole.GetInstance();

            libtcodWrapper.KeyPress key;
            do
            {
                rootConsole.Clear();
                rootConsole.PrintLine("Hello World", 10, 10, libtcodWrapper.LineAlignment.Center);
                rootConsole.Flush();

                // For Message Pumping
                key = libtcodWrapper.Keyboard.CheckForKeypress(libtcodWrapper.KeyPressType.Pressed);
            }
            while (!rootConsole.IsWindowClosed() && key.Character != 'q');
        }
    }
}
