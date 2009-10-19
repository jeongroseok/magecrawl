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

        private const int startingX = 51;
        private const int infoWidth = 29;
        private const int infoHeight = 60;
        public void Draw(Console screen, IPlayer player)
        {
            screen.DrawFrame(startingX, 0, infoWidth, infoHeight, true);
            screen.PrintLine(player.Name, startingX + (infoWidth / 2), 1, LineAlignment.Center);
        }
    }
}
