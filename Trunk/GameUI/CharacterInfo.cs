using System.Collections.Generic;
using System.Text;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI
{
    public class CharacterInfo
    {
        public CharacterInfo()
        {
        }

        private const int StartingX = UIHelper.MapWidth;
        private const int InfoWidth = UIHelper.CharInfoWidth;
        private const int InfoHeight = UIHelper.CharInfoHeight;

        private const int ScreenCenter = StartingX + (InfoWidth / 2);
        public void Draw(Console screen, IGameEngine engine, IPlayer player)
        {
            screen.DrawFrame(StartingX, 0, InfoWidth, InfoHeight, true);
            screen.PrintLine(player.Name, ScreenCenter, 1, LineAlignment.Center);

            string hpString = string.Format("HP: {0}/{1}", player.CurrentHP, player.MaxHP);
            screen.PrintLine(hpString, StartingX + 2, 2, LineAlignment.Left);
            string magicString = string.Format("Magic {0}/{1}", player.CurrentMP, player.MaxMP);
            screen.PrintLine(magicString, StartingX + 2, 3, LineAlignment.Left);

            screen.PrintLine("Status Effects:", StartingX + 2, 5, LineAlignment.Left);
            StringBuilder statusEffects = new StringBuilder();
            foreach (string s in player.StatusEffects)
            {
                statusEffects.Append(s + " ");
            }

            // TODO - What happens if this is more then 2 lines worth?
            screen.PrintLineRect(statusEffects.ToString(), StartingX + 2, 6, InfoWidth - 4, 2, LineAlignment.Left);
            
            if (Preferences.Instance.DebuggingMode)
            {
                string level = engine.CurrentLevel.ToString();
                screen.PrintLine(level, 52, 56, LineAlignment.Left);

                string position = player.Position.ToString();
                screen.PrintLine(position, 52, 57, LineAlignment.Left);

                string fps = TCODSystem.FPS.ToString();
                screen.PrintLine(fps, 52, 58, LineAlignment.Left);
            }
        }
    }
}
