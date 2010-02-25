using System.Collections.Generic;
using System.IO;
using libtcodWrapper;

namespace Magecrawl
{
    internal class WelcomeWindow : System.IDisposable
    {
        public enum SelectedOption
        {
            NewGame = 0,
            Load = 1,
            Quit = 2
        }

        public struct Result
        {
            public SelectedOption Choice;
            public string CharacterName;
        }

        private enum MagicTypes
        {
            Fire,
            Water,
            Air,
            Earth,
            Light,
            Darkness, 
            Arcane
        }

        private RootConsole m_console;
        private TCODRandom m_random;
        private Dictionary<MagicTypes, string> m_flavorText;
        private List<string> m_menuItemList;
        private SelectedOption m_menuItemPointedTo;
        private MagicTypes m_currentElementPointedTo;
        private Result m_windowResult;
        private List<string> m_fileNameList;
        private string m_fileInput = string.Empty;
        private bool m_loopFinished;

        private const int LengthOfEachElement = 180;
        private const int NumberOfSaveFilesToList = 15;
        private const int TextEntryLength = 15;

        // What "frame" are we on (draw request).
        // We only move the element select each LengthOfEachElement
        private int m_frame = 0;

        public WelcomeWindow()
        {
            m_console = libtcodWrapper.RootConsole.GetInstance();
            m_random = new TCODRandom();
            m_windowResult = new Result();

            m_currentElementPointedTo = (MagicTypes)m_random.GetRandomInt(0, 6);    // Pick a random element to start on.

            m_flavorText = new Dictionary<MagicTypes, string>();
            m_flavorText[MagicTypes.Fire] = "Ignis: Passionate fire-based magic. Aggressive front loaded damage. Weaker in magic requiring more finesse.";
            m_flavorText[MagicTypes.Water] = "Potizo: As seen in the seas, water can both protect and destory life with ease. Icey effects can also slow and disable.";
            m_flavorText[MagicTypes.Air] = "Aeras: Born from the wind, air-based magic can enchant, entice, and when necessary strike with thunder and hail.";
            m_flavorText[MagicTypes.Earth] = "Choma: Sturdy and dependable, the earth excels in defense and striking nearby foes with the force of a landslide.";
            m_flavorText[MagicTypes.Light] = "Agios: Gifted from the Creator. Best at mending wounds, summoning aid and protection. Strong at smiting unholy abominations.";
            m_flavorText[MagicTypes.Darkness] = "Anosios: Drawn from a pure lust for power. A mockery of Agios, capable of producing much pain, summoning unholy aid, and raising the dead to serve.";
            m_flavorText[MagicTypes.Arcane] = "Apokryfos: Magic born from the fabric of reality. Infinitely flexible and neutral to all other school of magic.";

            m_menuItemList = new List<string>() { "New Game", "Load Game", "Quit" };
            m_menuItemPointedTo = SelectedOption.NewGame;
        }

        public void Dispose()
        {
            if (m_random != null)
                m_random.Dispose();
            m_random = null;
        }

        public Result Run()
        {
            m_loopFinished = false;
            m_console.ForegroundColor = ColorPresets.WhiteSmoke;
            m_console.BackgroundColor = ColorPresets.Black;

            do
            {
                PassNewFrame();

                m_console.Clear();

                m_console.DrawFrame(0, 0, RootConsole.Width, RootConsole.Height, true);
                m_console.PrintLine("Welcome To Magecrawl", RootConsole.Width / 2, 1, LineAlignment.Center);
                m_console.PrintLine("Tech Demo III", RootConsole.Width / 2, 3, LineAlignment.Center);

                DrawMenu();

                DrawFileEntry();

                m_console.Flush();

                HandleKeystroke();
            }
            while (!m_loopFinished && !m_console.IsWindowClosed());
            return m_windowResult;
        }

        private void PassNewFrame()
        {
            if (m_frame > LengthOfEachElement)
            {
                IncrementElementPointedTo();
                m_frame = 0;
            }
            else
            {
                m_frame++;
            }
        }       

        private void HandleKeystroke()
        {
            KeyPress k = libtcodWrapper.Keyboard.CheckForKeypress(KeyPressType.Pressed);
            if (IsKeyCodeOfCharacter(k))
            {
                HandleCharacterInTextbox(ref m_fileInput, TextEntryLength - 2, k);
            }
            else
            {
                switch (k.KeyCode)
                {
                    case KeyCode.TCODK_BACKSPACE:
                        HandleBackspaceInTextbox(ref m_fileInput);
                        break;
                    case KeyCode.TCODK_UP:
                        if (m_menuItemPointedTo > 0)
                            m_menuItemPointedTo--;
                        break;
                    case KeyCode.TCODK_DOWN:
                        if ((int)m_menuItemPointedTo < m_menuItemList.Count - 1)
                            m_menuItemPointedTo++;
                        break;
                    case KeyCode.TCODK_ENTER:
                    case KeyCode.TCODK_KPENTER:
                    {
                        switch (m_menuItemPointedTo)
                        {
                            case SelectedOption.NewGame:
                            {
                                if (!DoesSaveFileExist())
                                {
                                    m_windowResult.Choice = SelectedOption.NewGame;
                                    m_windowResult.CharacterName = m_fileInput;
                                    m_loopFinished = true;
                                }
                                break;
                            }
                            case SelectedOption.Load:
                            {
                                if (DoesSaveFileExist())
                                {
                                    m_windowResult.Choice = SelectedOption.Load;
                                    m_windowResult.CharacterName = m_fileInput;
                                    m_loopFinished = true;
                                }
                                break;
                            }
                            case SelectedOption.Quit:
                            {
                                m_windowResult.Choice = SelectedOption.Quit;
                                m_loopFinished = true;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void DrawFileEntry()
        {
            if (m_menuItemPointedTo == SelectedOption.NewGame)
            {
                int fileEntryOffset = 22;
                m_console.PrintLine("Enter Name", 3, fileEntryOffset, LineAlignment.Left);
                m_console.DrawFrame(2, fileEntryOffset + 1, TextEntryLength + 2, 3, true);
                m_console.PrintLineRect(m_fileInput, 4, fileEntryOffset + 2, TextEntryLength - 2, 1, LineAlignment.Left);

                if (libtcodWrapper.TCODSystem.ElapsedMilliseconds % 1500 < 700)
                    m_console.SetCharBackground(4 + m_fileInput.Length, fileEntryOffset + 2, libtcodWrapper.TCODColorPresets.Gray);
            }
            else
            {
                m_fileInput = string.Empty;
            }
        }

        private void DrawMenu()
        {
            const int MenuUpperX = 4;
            const int MenuUpperY = 14;
            m_console.ForegroundColor = ColorPresets.WhiteSmoke;
            m_console.DrawFrame(MenuUpperX - 2, MenuUpperY - 2, 17, (m_menuItemList.Count * 2) + 3, true);
            for (int i = 0; i < m_menuItemList.Count; i++)
            {
                if (i == (int)m_menuItemPointedTo)
                {
                    m_console.ForegroundColor = ColorPresets.WhiteSmoke;
                    m_console.BackgroundColor = ColorPresets.Blue;
                }
                else
                {
                    m_console.ForegroundColor = ColorPresets.Gray;
                    m_console.BackgroundColor = ColorPresets.Black;
                }

                m_console.PrintLine(m_menuItemList[i], MenuUpperX, MenuUpperY + (i * 2), Background.Set, LineAlignment.Left);
            }
            m_console.ForegroundColor = ColorPresets.WhiteSmoke;
            m_console.BackgroundColor = ColorPresets.Black;
        }

        private void IncrementElementPointedTo()
        {
            m_currentElementPointedTo++;
            if (m_currentElementPointedTo > MagicTypes.Arcane)
                m_currentElementPointedTo = MagicTypes.Fire;
        }

        public bool DoesSaveFileExist()
        {
            if (m_fileInput.Length == 0)
                return false;
            return File.Exists(m_fileInput + ".sav");
        }

        public static List<string> GetListOfSaveFiles()
        {
            string[] fullPathList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sav");
            List<string> nameList = new List<string>();
            foreach (string s in fullPathList)
                nameList.Add(Path.GetFileNameWithoutExtension(s));
            return nameList;
        }

        private static bool IsKeyCodeOfCharacter(KeyPress k)
        {
            return k.KeyCode == KeyCode.TCODK_CHAR || k.KeyCode == KeyCode.TCODK_0 || k.KeyCode == KeyCode.TCODK_1
                || k.KeyCode == KeyCode.TCODK_2 || k.KeyCode == KeyCode.TCODK_3 || k.KeyCode == KeyCode.TCODK_4
                || k.KeyCode == KeyCode.TCODK_5 || k.KeyCode == KeyCode.TCODK_6 || k.KeyCode == KeyCode.TCODK_7
                || k.KeyCode == KeyCode.TCODK_8 || k.KeyCode == KeyCode.TCODK_9;
        }

        private static void HandleCharacterInTextbox(ref string buffer, int maxLength, KeyPress key)
        {
            if (buffer.Length < maxLength)
            {
                bool validCharacter = !(((char)key.Character).ToString().IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1);
                if (validCharacter)
                    buffer = buffer + (char)key.Character;
            }
        }

        private static void HandleBackspaceInTextbox(ref string buffer)
        {
            if (buffer.Length > 0)
                buffer = buffer.Substring(0, buffer.Length - 1);
        }
    }
}
