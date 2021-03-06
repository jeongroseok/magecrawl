using System;
using System.Collections.Generic;
using System.IO;
using libtcod;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Utilities;
using Magecrawl.Utilities;

namespace Magecrawl
{
    internal class WelcomeWindow
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
        private const int LengthOfEachElement = 1000;
        
        private const int SpellListOffset = 42;
        private const int EntryOffset = 8;
        private const int LoadFileListOffset = EntryOffset + 4;

        private const int EntryWidth = 2;
        private const int NumberOfSaveFilesToList = 18;
        private const int NumberOfSpells = 6;
        private const int TextEntryLength = 23; // This should be odd to make centering easy

        private DialogColorHelper m_dialogHelper;
        private TCODConsole m_console;
        private Random m_random;
        private Dictionary<MagicTypes, string> m_flavorText;
        private Dictionary<MagicTypes, List<string>> m_spellLists;
        private Dictionary<MagicTypes, string> m_mainText;
        private MagicTypes m_currentElementPointedTo;
        private Result m_windowResult;
        private List<string> m_fileNameList;
        private string m_fileInput = "";
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
            m_console = TCODConsole.root;
            m_random = new Random();
            m_windowResult = new Result();

            m_currentElementPointedTo = (MagicTypes)m_random.getInt(0, 6);    // Pick a random element to start on.

            m_flavorText = new Dictionary<MagicTypes, string>();
            m_flavorText[MagicTypes.Fire] = "Passionate fire-based magic. Aggressive front loaded damage. Weaker in magic requiring more finesse.";
            m_flavorText[MagicTypes.Water] = "As seen in the seas, water can both protect and destory life with ease. Icey effects also slow and disable.";
            m_flavorText[MagicTypes.Air] = "Born from the wind, air-based magic can enchant, entice, and when necessary strike with thunder and hail.";
            m_flavorText[MagicTypes.Earth] = "Sturdy and dependable, the earth excels in defense and striking nearby foes with the force of a landslide.";
            m_flavorText[MagicTypes.Light] = "Gifted from the Creator. Light magic is best at mending wounds, summoning aid and protection. Strong at smiting unholy abominations.";
            m_flavorText[MagicTypes.Darkness] = "Darkness magic is drawn from a pure lust for power. A mockery of the light, capable of producing much pain, summoning forbidden aid, and raising the dead to serve.";
            m_flavorText[MagicTypes.Arcane] = "Arcane magic is born from altering the fabric of reality. Infinitely flexible and neutral to all other school of magic.";

            m_mainText = new Dictionary<MagicTypes, string>();
            m_mainText[MagicTypes.Fire] = "It is our passions that seperate us from the other schools, our passion for life, our passion for knowledges, our passion for power.\n\nIt is your fiery passion that brought you to this school. Your instructors will shape your unrefined passion into power. Power to shape even the very flames beneath this mountain. \n\nTo be a pyromancer is to be channel forces so great as to be nearly out of control and yet hold onto it with your very will.\n\nRemember this as you enter these halls, as many have before you.\n\n\n\n\n\n\nSenior Pyromancer Nathan\nWelcoming Ceremony\n372 CT";
            m_mainText[MagicTypes.Water] = "And this exhibit is the famous painting of the battle of Hillsbrad Harbor. During the 2nd revolution in CT 291, this town was stormed by a small navy backed up by a large elemental force, as you can see here.\n\nWater wisps, ice demons, and even elder water elementals were summoned by the Archmage Danel II. Look at the exquisite detail of the background storm. The full force a hurricane force winds were leveled against the defenders, and is captured here.\n\nIt was painted at the height of the artist's career. Now on the left is...\n\n\n\n\n\nExcerpt from a tour\nRoyal Museum of Art, Dalundor\nCT 382";
            m_mainText[MagicTypes.Air] = "While traveling via the sea, many remember to visit the Brotherhood of Water for charms and scrolls to speed their journey. You would be wise not to forget the power of the wind. Even with a calm sea, the wind is what truly carries you.\nSome the the product we offer the public inclued:\n\n\nScroll of Strengthen Wind......50G\n\nScroll of Witch Wind..........100G\n\nWand of Calm Storm............275G\n\nWand of Lightning Bolt........500G\n\nServices of Air Mage......500G/day\n\n\nCome see us today!\n\nAirbone Services\nLocated in Arcane district next to the west gate.";
            m_mainText[MagicTypes.Earth] = "Tips when traveling to planes of Earth\n\n* 37 planes of earth have been mapped sufficiently well enough that bringing a map will do more help than harm.\n\n* Seven of these planes are partially molten, so bring a fire mage or resist elements scrolls.\n\n* While many will travel in search of precious metals and gems, removal of large quantites of materal can rouse angry elementals.\n\n* Bring scrolls of fresh air and potions of burrowing in case of cave-ins.\n\n* Remember a light source that doesn't require air, such as glowstones or coldfire torches.\n\n\nGuild of Earth Mages";
            m_mainText[MagicTypes.Light] = "In the beginning the Creator created many worlds. Some demi-planes of raw elements, others fully formed and seeded with life. While mages from all schools have explored a few of these planes, many questions remain.\n\nWhy are some more formed than others? Why did the Creator leave His fingerprints on creation so obviously, and then never act again?\n\nThe tenants of the school of light magic are based upon what we can glean about the Creator and planes of positive energy.\n\nHowever, unless the Creator returns to impart knowledge about itself, there is precious little we can know for certain.\n\n\n\nThoughts on Creation\nAugus\nCT 169";
            m_mainText[MagicTypes.Darkness] = "Jonathan,\n\n     You were right, those hidden manuscripts we found were truthful. While I have not found the tomb yet, this swamp is full of power. The dead walk, wondering (patrolling?) near this bog, the very air is filled with a dread power stronger than anything at the academy.\n\nOnce I find the tomb, the spellbooks of the Lich Algmon shall be ours. With them, the power to shape the world. Those fools who speak of restraint or treaties lack the will to act. That is one thing we do not lack.\n\nAs soon as you can slip away, locate me using the charm attached. We can continue the search.\n\n\n\nMarie\nCT 381";
            m_mainText[MagicTypes.Arcane] = "Magic in its purest form is a sentient being pressing their will against nature. More accurately, their will distorts the weave of creation. By understanding this weave, a student of the arcane can wield force alone as their weapon, as opposed to crude elemental magics.\n\nWith study, the amount of distortion one can force into the weave before it \"snaps\" back and ruins a spell increases.\n\nThis textbook will provide an introduction to Arcane magic. The study of the arcane provides both an excellent foundation for future studies in other areas and a path to high magic in itself.\n\n\nModern Exploration of the Weave: An Introduction to Modern Arcane theory\nPage 3\nDavid C. Fellow";

            m_spellLists = new Dictionary<MagicTypes, List<string>>();
            m_spellLists[MagicTypes.Fire] = new List<string>() { "Firebolt", "Warmth", "Fireblast", "Fireball", "Wall Of Fire", "Firestorm" };
            m_spellLists[MagicTypes.Water] = new List<string>() { "Cold Snap", "Icy Armor", "Frigid Aura", "Ice Bolt", "Shatter", "Cone Of Cold" };
            m_spellLists[MagicTypes.Air] = new List<string>() { "Shock", "Swiftness", "Confuse", "Lightning Bolt", "Storm Armor", "Hurricane" };
            m_spellLists[MagicTypes.Earth] = new List<string>() { "Stone Armor", "Aftershock", "Strength Of The Earth", "Stoneskin", "Summon Earth Elemental", "Earthquake" };
            m_spellLists[MagicTypes.Light] = new List<string>() { "Mend", "Smite", "Armor of Light", "Heal", "Summon Archon", "Wrath" };
            m_spellLists[MagicTypes.Darkness] = new List<string>() { "Pain", "Drain Life", "Raise Dead", "Darkness", "Summon Wraith", "Lich Form" };
            m_spellLists[MagicTypes.Arcane] = new List<string>() { "Force Bolt", "Arcane Armor", "Blink", "Slow", "Destructive Teleport", "Wave of Force" };

            GetListOfSaveFiles();
            m_lowerRange = 0;
            m_cursorPosition = -1;
            m_isScrollingNeeded = m_fileNameList.Count > NumberOfSaveFilesToList;
        }

        public Result Run()
        {
            m_loopFinished = false;
            m_console.setForegroundColor(UIHelper.ForegroundColor);
            m_console.setBackgroundColor(ColorPresets.Black);

            do
            {
                PassNewFrame();

                m_console.clear();

                m_console.printFrame(0, 0, m_console.getWidth(), m_console.getHeight(), true);
                m_console.printEx(m_console.getWidth() / 2, 2, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, "Welcome To Magecrawl");
                m_console.printEx(m_console.getWidth() / 2, 4, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, "Tech Demo III v2");

                DrawLoadFilesMenu();
                DrawFileEntry();
                DrawGraphic();
                DrawSpellList();

                TCODConsole.flush();

                HandleKeystroke();
            }
            while (!m_loopFinished && !TCODConsole.isWindowClosed());
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
            TCODKey k = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed);
            if (IsKeyCodeOfCharacter(k))
            {
                HandleCharacterInTextbox(k);
            }
            else
            {
                switch (k.KeyCode)
                {
                    case TCODKeyCode.Backspace:
                        HandleBackspaceInTextbox();
                        break;
                    case TCODKeyCode.Escape:
                        m_windowResult.Quitting = true;
                        m_loopFinished = true;
                        break;
                    case TCODKeyCode.Up:
                        MoveInventorySelection(true);
                        break;
                    case TCODKeyCode.Down:
                        MoveInventorySelection(false);
                        break;
                    case TCODKeyCode.Enter:
                    case TCODKeyCode.KeypadEnter:
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
                m_fileInput = "";
            }
        }

        private void DrawGraphic()
        {
            const int GraphicTextX = 32;
            const int GraphicTextY = 42;
            const int GraphicTextWidth = 40;
            const int GraphicTextHeight = 10;

            int numberOfLines = m_console.printRect(GraphicTextX + 2, GraphicTextY + 2, GraphicTextWidth - 4, GraphicTextHeight, m_flavorText[m_currentElementPointedTo]);
            m_console.printFrame(GraphicTextX, GraphicTextY, GraphicTextWidth, numberOfLines + 4, false);

            const int GraphicX = 32;
            const int GraphicY = 8;

            numberOfLines = m_console.printRect(GraphicX + 2, GraphicY + 2, GraphicTextWidth - 4, 32, m_mainText[m_currentElementPointedTo]);
            m_console.printFrame(GraphicX, GraphicY, GraphicTextWidth, 32, false);
        }

        private void DrawSpellList()
        {
            m_console.printFrame(EntryWidth, SpellListOffset - 3, TextEntryLength + 4, 3, true);
            m_console.printEx(EntryWidth + (TextEntryLength / 2) + 2, SpellListOffset - 2, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, m_currentElementPointedTo.ToString() + " Spells");

            m_console.printFrame(EntryWidth, SpellListOffset, TextEntryLength + 4, 15, true);

            int numberOfSpellsToShow = m_frame / (LengthOfEachElement / NumberOfSpells);
            numberOfSpellsToShow = System.Math.Min(numberOfSpellsToShow + 1, NumberOfSpells);
            for (int i = 0; i < numberOfSpellsToShow; ++i)
            {
                m_console.printEx(EntryWidth + (TextEntryLength / 2) + 2, SpellListOffset + 2 + (2 * i), TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, m_spellLists[m_currentElementPointedTo][i]); 
            }
        }

        private void DrawLoadFilesMenu()
        {
            if (SaveFilesExist())
            {
                m_dialogHelper.SaveColors(m_console);
                m_higherRange = m_isScrollingNeeded ? m_lowerRange + NumberOfSaveFilesToList : m_fileNameList.Count;

                m_console.printFrame(EntryWidth, LoadFileListOffset, TextEntryLength + 4, NumberOfSaveFilesToList + 4, true);

                int numberToList = (m_fileNameList.Count < NumberOfSaveFilesToList) ? m_fileNameList.Count : NumberOfSaveFilesToList;

                int positionalOffsetFromTop = 0;
                for (int i = m_lowerRange; i < m_higherRange; i++)
                {
                    m_dialogHelper.SetColors(m_console, i == m_cursorPosition, true);
                    m_console.printRect(EntryWidth + 2, LoadFileListOffset + 2 + positionalOffsetFromTop, TextEntryLength + 2, 1, m_fileNameList[i]);
                    positionalOffsetFromTop++;
                }

                if (m_lowerRange != 0)
                {
                    m_dialogHelper.SetColors(m_console, false, true);
                    m_console.printRect(EntryWidth + 2, LoadFileListOffset + 1, TextEntryLength + 2, 1, "..more..");
                }

                if (m_higherRange < m_fileNameList.Count)
                {
                    m_dialogHelper.SetColors(m_console, false, true);
                    m_console.printRect(EntryWidth + 2, LoadFileListOffset + 2 + numberToList, TextEntryLength + 2, 1, "..more..");
                }
                m_dialogHelper.ResetColors(m_console);
            }
        }

        private void DrawFileEntry()
        {
            if (m_cursorPosition == -1)
                m_console.print(EntryWidth + 1, EntryOffset, "Enter Name");

            m_console.printFrame(EntryWidth, EntryOffset + 1, TextEntryLength + 4, 3, true);
            m_console.printRect(EntryWidth + 2, EntryOffset + 2, TextEntryLength, 1, m_fileInput);

            if (m_cursorPosition == -1 && m_fileInput.Length < TextEntryLength)
            {
                if (TCODSystem.getElapsedMilli() % 1500 < 700)
                    m_console.setCharBackground(EntryWidth + 2 + m_fileInput.Length, EntryOffset + 2, TCODColor.grey);
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
            if (Directory.Exists(saveGameDirectory))
            {
                string[] fullPathList = Directory.GetFiles(saveGameDirectory, "*.sav");
                foreach (string s in fullPathList)
                    m_fileNameList.Add(Path.GetFileNameWithoutExtension(s));
            }
        }

        private static bool IsKeyCodeOfCharacter(TCODKey k)
        {
            return k.KeyCode == TCODKeyCode.Char || k.KeyCode == TCODKeyCode.Zero || k.KeyCode == TCODKeyCode.One
                || k.KeyCode == TCODKeyCode.Two || k.KeyCode == TCODKeyCode.Three || k.KeyCode == TCODKeyCode.Four
                || k.KeyCode == TCODKeyCode.Five || k.KeyCode == TCODKeyCode.Six || k.KeyCode == TCODKeyCode.Seven
                || k.KeyCode == TCODKeyCode.Eight || k.KeyCode == TCODKeyCode.Nine;
        }

        private void HandleCharacterInTextbox(TCODKey key)
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
