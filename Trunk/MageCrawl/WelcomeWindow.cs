﻿using System.Collections.Generic;
using System.IO;
using libtcodWrapper;
using Magecrawl.GameUI;
using Magecrawl.Utilities;

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

        private const int LengthOfEachElement = 180;
        private const int NumberOfSaveFilesToList = 15;
        private const int TextEntryLength = 15;
        private const int EntryOffset = 22;

        private DialogColorHelper m_dialogHelper;
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
            m_console.ForegroundColor = UIHelper.ForegroundColor;
            m_console.BackgroundColor = ColorPresets.Black;

            do
            {
                PassNewFrame();

                m_console.Clear();

                m_console.DrawFrame(0, 0, RootConsole.Width, RootConsole.Height, true);
                m_console.PrintLine("Welcome To Magecrawl", RootConsole.Width / 2, 1, LineAlignment.Center);
                m_console.PrintLine("Tech Demo III", RootConsole.Width / 2, 3, LineAlignment.Center);

                DrawMenu();
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
                        m_fileInput = string.Empty;
                        break;
                    case KeyCode.TCODK_DOWN:
                        if ((int)m_menuItemPointedTo < m_menuItemList.Count - 1)
                            m_menuItemPointedTo++;
                        m_fileInput = string.Empty;
                        break;
                    case KeyCode.TCODK_ENTER:
                    case KeyCode.TCODK_KPENTER:
                    {
                        switch (m_menuItemPointedTo)
                        {
                            case SelectedOption.NewGame:
                            {
                                if (m_fileInput.Length != 0 && !DoesInputNameSaveFileExist())
                                {
                                    m_windowResult.Choice = SelectedOption.NewGame;
                                    m_windowResult.CharacterName = m_fileInput;
                                    m_loopFinished = true;
                                }
                                break;
                            }
                            case SelectedOption.Load:
                            {
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

        private void DrawLoadFilesMenu()
        {
            if (m_menuItemPointedTo == SelectedOption.Load && SaveFilesExist())
            {
                m_console.PrintLine("Save Files", 3, EntryOffset, LineAlignment.Left);
                //m_console.DrawFrame(2, EntryOffset + 1, TextEntryLength + 2, 3, true);
                int sizeOfFrame = (m_fileNameList.Count < NumberOfSaveFilesToList) ? m_fileNameList.Count : NumberOfSaveFilesToList + 1;
                int numberToList = (m_fileNameList.Count < NumberOfSaveFilesToList) ? m_fileNameList.Count : NumberOfSaveFilesToList;

                m_console.DrawFrame(8, EntryOffset + 1, TextEntryLength + 2, sizeOfFrame + 2, true);
                for (int i = 0; i < numberToList; i++)
                    m_console.PrintLineRect(m_fileNameList[i], 4, EntryOffset + 2 + i, TextEntryLength - 4, 1, LineAlignment.Left);

                if (numberToList == NumberOfSaveFilesToList)
                {
                    m_console.PrintLineRect("                   ", 3, EntryOffset + 6 + numberToList, TextEntryLength - 4, 1, LineAlignment.Left);
                    m_console.PrintLineRect("..more..", 4, EntryOffset + 6 + numberToList, TextEntryLength - 4, 1, LineAlignment.Left);
                }	
            }
        }

        private void DrawFileEntry()
        {
            if (m_menuItemPointedTo == SelectedOption.NewGame)
            {
                m_console.PrintLine("Enter Name", 3, EntryOffset, LineAlignment.Left);
                m_console.DrawFrame(2, EntryOffset + 1, TextEntryLength + 2, 3, true);
                m_console.PrintLineRect(m_fileInput, 4, EntryOffset + 2, TextEntryLength - 2, 1, LineAlignment.Left);

                if (libtcodWrapper.TCODSystem.ElapsedMilliseconds % 1500 < 700)
                    m_console.SetCharBackground(4 + m_fileInput.Length, EntryOffset + 2, libtcodWrapper.TCODColorPresets.Gray);
            }
        }

        private void DrawMenu()
        {
            const int MenuUpperX = 4;
            const int MenuUpperY = 14;
            
            m_dialogHelper.SaveColors(m_console);

            m_console.DrawFrame(MenuUpperX - 2, MenuUpperY - 2, 17, (m_menuItemList.Count * 2) + 3, true);
            for (int i = 0; i < m_menuItemList.Count; i++)
            {
                bool isEnabled = i == (int)SelectedOption.Load ? SaveFilesExist() : true;
                m_dialogHelper.SetColors(m_console, i == (int)m_menuItemPointedTo, isEnabled);
                m_console.PrintLine(m_menuItemList[i], MenuUpperX, MenuUpperY + (i * 2), Background.Set, LineAlignment.Left);
            }
            m_dialogHelper.ResetColors(m_console);
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
            return File.Exists(m_fileInput + ".sav");
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
