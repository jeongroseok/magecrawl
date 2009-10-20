using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameUI
{
    public class CharacterInfo
    {
        public CharacterInfo()
        {
        }

        private const int StartingX = 51;
        private const int InfoWidth = 29;
        private const int InfoHeight = 60;
        public void Draw(Console screen, IPlayer player)
        {
            screen.DrawFrame(StartingX, 0, InfoWidth, InfoHeight, true);
            screen.PrintLine(player.Name, StartingX + (InfoWidth / 2), 1, LineAlignment.Center);
            
            string fps = TCODSystem.FPS.ToString();
            screen.PrintLine(fps, 52, 58, LineAlignment.Left);
        }
    }
}
