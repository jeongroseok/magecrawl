using System.Collections.Generic;
using System.IO;
using libtcodWrapper;
using Magecrawl.GameUI;
using Magecrawl.Utilities;

namespace Magecrawl
{
    internal class WelcomeWindow : System.IDisposable
    {
        public struct Result
        {
            public bool Quitting;
            public bool LoadCharacter;
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

        private const int ScrollAmount = 3;
        private const int LengthOfEachElement = 180;
        private const int TextEntryLength = 20;
        private const int EntryOffset = 36;
        private const int LoadFileListOffset = 40;
        private const int NumberOfSaveFilesToList = UIHelper.ScreenHeight - LoadFileListOffset - 5;

        private DialogColorHelper m_dialogHelper;
        private RootConsole m_console;
        private TCODRandom m_random;
        private Dictionary<MagicTypes, string> m_flavorText;
        private MagicTypes m_currentElementPointedTo;
        private Result m_windowResult;
        private List<string> m_fileNameList;
        private string m_fileInput = string.Empty;
        private bool m_loopFinished;

        private int m_lowerRange;                       // If we're scrolling, the loweset number item to show
        private int m_higherRange;                      // Last item to show
        private bool m_isScrollingNeeded;               // Do we need to scroll at all?
        private int m_cursorPosition;                   // What item is the cursor on, -1 is the text box
        
        // What "frame" are we on (draw request).
        // We only move the element select each LengthOfEachElement
        private int m_frame = 0;

        public WelcomeWindow()
        {
            m_dialogHelper = new DialogColorHelper();
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

            GetListOfSaveFiles();
            m_lowerRange = 0;
            m_cursorPosition = -1;
            m_isScrollingNeeded = m_fileNameList.Count > NumberOfSaveFilesToList;
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
            m_console.ForegroundColor = UIHelper.ForegroundColor;
            m_console.BackgroundColor = ColorPresets.Black;

            do
            {
                PassNewFrame();

                m_console.Clear();

                m_console.DrawFrame(0, 0, RootConsole.Width, RootConsole.Height, true);
                m_console.PrintLine("Welcome To Magecrawl", RootConsole.Width / 2, 1, LineAlignment.Center);
                m_console.PrintLine("Tech Demo III", RootConsole.Width / 2, 3, LineAlignment.Center);

                DrawLoadFilesMenu();
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
                HandleCharacterInTextbox(k);
            }
            else
            {
                switch (k.KeyCode)
                {
                    case KeyCode.TCODK_BACKSPACE:
                        HandleBackspaceInTextbox();
                        break;
                    case KeyCode.TCODK_ESCAPE:
                        m_windowResult.Quitting = true;
                        m_loopFinished = true;
                        break;
                    case KeyCode.TCODK_UP:
                        MoveInventorySelection(true);
                        break;
                    case KeyCode.TCODK_DOWN:
                        MoveInventorySelection(false);
                        break;
                    case KeyCode.TCODK_ENTER:
                    case KeyCode.TCODK_KPENTER:
                    {
                        if (m_cursorPosition == -1)
                        {
                            if (m_fileInput.Length > 0)
                            {
                                m_windowResult.LoadCharacter = DoesInputNameSaveFileExist();
                                m_windowResult.Quitting = false;
                                m_windowResult.CharacterName = m_fileInput;
                                m_loopFinished = true;
                            }
                        }
                        else
                        {
                            m_windowResult.LoadCharacter = true;
                            m_windowResult.Quitting = false;
                            m_windowResult.CharacterName = m_fileNameList[m_cursorPosition];
                            m_loopFinished = true;
                        }
                        break;
                    }
                }
            }
        }


        internal void MoveInventorySelection(bool movingUp)
        {
            if (movingUp)
            {
                if (m_cursorPosition >= 0)
                {
                    if (m_isScrollingNeeded && (m_cursorPosition == m_lowerRange))
                    {
                        m_lowerRange -= ScrollAmount;
                        if (m_lowerRange < 0)
                            m_lowerRange = 0;
                    }
                    m_cursorPosition--;
                }
            }
            if (!movingUp && m_cursorPosition < m_fileNameList.Count - 1)
            {
                // If we need scrolling and we're pointed at the end of the list and there's more to show.
                if (m_isScrollingNeeded && (m_cursorPosition == (m_lowerRange - 1 + NumberOfSaveFilesToList)) && (m_lowerRange + NumberOfSaveFilesToList < m_fileNameList.Count))
                {
                    m_lowerRange += ScrollAmount;
                    if ((m_lowerRange + NumberOfSaveFilesToList) > m_fileNameList.Count)
                        m_lowerRange = m_fileNameList.Count - NumberOfSaveFilesToList;

                    m_cursorPosition++;
                }
                else
                {
                    if ((m_cursorPosition + 1) < m_fileNameList.Count)
                        m_cursorPosition++;
                }
                m_fileInput = string.Empty;
            }
        }

        private void DrawLoadFilesMenu()
        {
            if (SaveFilesExist())
            {
                m_dialogHelper.SaveColors(m_console);
                m_higherRange = m_isScrollingNeeded ? m_lowerRange + NumberOfSaveFilesToList : m_fileNameList.Count;

                m_console.DrawFrame(1, LoadFileListOffset, TextEntryLength + 4, UIHelper.ScreenHeight - LoadFileListOffset - 1, true);

                int numberToList = (m_fileNameList.Count < NumberOfSaveFilesToList) ? m_fileNameList.Count : NumberOfSaveFilesToList;

                int positionalOffsetFromTop = 0;
                for (int i = m_lowerRange; i < m_higherRange; i++)
                {
                    m_dialogHelper.SetColors(m_console, i == m_cursorPosition, true);
                    m_console.PrintLineRect(m_fileNameList[i], 3, LoadFileListOffset + 2 + positionalOffsetFromTop, TextEntryLength + 2, 1, LineAlignment.Left);
                    positionalOffsetFromTop++;
                }

                if (m_lowerRange != 0)
                {
                    m_dialogHelper.SetColors(m_console, false, true);
                    m_console.PrintLineRect("..more..", 3, LoadFileListOffset + 1, TextEntryLength + 2, 1, LineAlignment.Left);
                }

                if (m_higherRange < m_fileNameList.Count)
                {
                    m_dialogHelper.SetColors(m_console, false, true);
                    m_console.PrintLineRect("..more..", 3, LoadFileListOffset + 2 + numberToList, TextEntryLength + 2, 1, LineAlignment.Left);
                }
                m_dialogHelper.ResetColors(m_console);
            }
        }

        private void DrawFileEntry()
        {
            if(m_cursorPosition == -1)
                m_console.PrintLine("Enter Name", 2, EntryOffset, LineAlignment.Left);

            m_console.DrawFrame(1, EntryOffset + 1, TextEntryLength + 4, 3, true);
            m_console.PrintLineRect(m_fileInput, 3, EntryOffset + 2, TextEntryLength, 1, LineAlignment.Left);

            if (m_cursorPosition == -1 && m_fileInput.Length < TextEntryLength)
            {
                if (libtcodWrapper.TCODSystem.ElapsedMilliseconds % 1500 < 700)
                    m_console.SetCharBackground(3 + m_fileInput.Length, EntryOffset + 2, libtcodWrapper.TCODColorPresets.Gray);
            }
        }

        private void IncrementElementPointedTo()
        {
            m_currentElementPointedTo++;
            if (m_currentElementPointedTo > MagicTypes.Arcane)
                m_currentElementPointedTo = MagicTypes.Fire;
        }

        private bool SaveFilesExist()
        {
            return m_fileNameList.Count > 0;
        }

        public bool DoesInputNameSaveFileExist()
        {
            return m_fileNameList.Exists(x => x == m_fileInput);
        }

        public void GetListOfSaveFiles()
        {
            string saveGameDirectory = Path.Combine(AssemblyDirectory.CurrentAssemblyDirectory, "Saves");
            m_fileNameList = new List<string>();
            string[] fullPathList = Directory.GetFiles(saveGameDirectory, "*.sav");
            foreach (string s in fullPathList)
                m_fileNameList.Add(Path.GetFileNameWithoutExtension(s));
        }

        private static bool IsKeyCodeOfCharacter(KeyPress k)
        {
            return k.KeyCode == KeyCode.TCODK_CHAR || k.KeyCode == KeyCode.TCODK_0 || k.KeyCode == KeyCode.TCODK_1
                || k.KeyCode == KeyCode.TCODK_2 || k.KeyCode == KeyCode.TCODK_3 || k.KeyCode == KeyCode.TCODK_4
                || k.KeyCode == KeyCode.TCODK_5 || k.KeyCode == KeyCode.TCODK_6 || k.KeyCode == KeyCode.TCODK_7
                || k.KeyCode == KeyCode.TCODK_8 || k.KeyCode == KeyCode.TCODK_9;
        }

        private void HandleCharacterInTextbox(KeyPress key)
        {
            if (m_cursorPosition == -1)
            {
                if (m_fileInput.Length < TextEntryLength)
                {
                    bool validCharacter = !(((char)key.Character).ToString().IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1);
                    if (validCharacter)
                        m_fileInput = m_fileInput + (char)key.Character;
                }
            }
        }

        private void HandleBackspaceInTextbox()
        {
            if (m_cursorPosition == -1)
            {
                if (m_fileInput.Length > 0)
                    m_fileInput = m_fileInput.Substring(0, m_fileInput.Length - 1);
            }
        }
    }
}
