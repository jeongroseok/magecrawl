using libtcod;
using Magecrawl.GameUI;
using Magecrawl.Utilities;
using System.Collections.Generic;

namespace Magecrawl
{
    internal class NewPlayerOptionsWindow
    {
        static TCODRandom s_random = new TCODRandom();

        private struct PlayerOption
        {
            public string Name;
            public string Description;
            public string OptionSelected;
        }

        private PlayerOption[] m_options = new PlayerOption[] { 
            new PlayerOption() { Name = "Scholar", Description = "Scholars are the researchers and historians of the arcane world. Spending much time in libraries and schools of magic, they have little practical knowledge in combat. However they compensate for this with a broad knowledge of arcane theory.                                                                               Scholars start clothed in linen robes and wielding a simple wooden staff, relying heavily on their magic to protect them.", OptionSelected = "Scholar"},
            new PlayerOption() { Name = "Scout", Description = "All orders of magi crave knowledge of the world around them and of possible threats. Mages, armed with spells of teleportation and scrying, make the perfect scout. While relying more on their armor and martial skills than the scholar, scouts still are mages first and foremost.                                                                                                   Scouts start with a set of light leather armor, and armed with a dagger and sling.", OptionSelected = "Scout"},
            new PlayerOption() { Name = "Templar", Description = "Magic can be devistating on the battlefield, with a mage worth a hundred common soldiers. Templars are trained in heavy armor and combat magic. Templar relay heavily on their equipment, using simple magic to best their opponent up close.                                                                                  Templar start with a full set of bronze armor along with a sword.", OptionSelected = "Templar"},
            new PlayerOption() { Name = "Random", Description = "Let the fates determine your background...", OptionSelected = (new List<string>() { "Scholar", "Scout", "Templar"}).Randomize()[0]}
            };

        private TCODConsole m_console;
        private int m_selected;
        private DialogColorHelper m_dialogColorHelper;

        internal NewPlayerOptionsWindow()
        {
            m_dialogColorHelper = new DialogColorHelper();
            m_console = TCODConsole.root;
            m_selected = 0;
        }

        internal string Run()
        {
            TCODKey k;
            do
            {
                m_console.printFrame(5, 5, UIHelper.ScreenWidth - 10, UIHelper.ScreenHeight - 10, true);

                string introString = "The path for a mage is open, unbound by any concept of \"class\".\n\nHowever the way in which one spent their years before journeying to the dungeon affects their starting equipment and skills.\n\nPlease select a background.";
                int introOffset = m_console.printRectEx(UIHelper.ScreenWidth / 2, 7, UIHelper.ScreenWidth - 17, 10, TCODBackgroundFlag.None, TCODAlignment.CenterAlignment, introString);

                int currentY = introOffset + 10;
                for (int i = 0; i < m_options.Length; ++i)
                {
                    if (i == m_selected)
                    {
                        m_dialogColorHelper.SaveColors(m_console);
                        m_dialogColorHelper.SetColors(m_console, true, true);
                        m_console.print(8, currentY, m_options[i].Name);
                        m_dialogColorHelper.ResetColors(m_console);
                    }
                    else
                    {
                        m_console.print(8, currentY, m_options[i].Name);
                    }

                    currentY += m_console.printRect(18, currentY, UIHelper.ScreenWidth - 7 - 18, 10, m_options[i].Description);
                    currentY += 3;
                }

                TCODConsole.flush();
                k = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed);

                if (k.KeyCode == TCODKeyCode.Up)
                {
                    if (m_selected > 0)
                        m_selected--;
                }
                else if (k.KeyCode == TCODKeyCode.Down)
                {
                    if (m_selected < m_options.Length - 1)
                        m_selected++;
                }
            }
            while (k.KeyCode != TCODKeyCode.Enter && k.KeyCode != TCODKeyCode.KeypadEnter && !TCODConsole.isWindowClosed());
            
            if (TCODConsole.isWindowClosed())
                return null;

            return m_options[m_selected].OptionSelected;
        }


    }
}
