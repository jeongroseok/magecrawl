using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Dialogs
{
    internal class HelpPainter : MapPainterBase
    {
        private bool m_enabled;
        private Dictionary<string, string> m_keyMappings;
        private DialogColorHelper m_dialogColorHelper;
        private bool m_creditsDone;
        private bool m_firstFrame;

        internal HelpPainter()
        {
            m_enabled = false;
            m_dialogColorHelper = new DialogColorHelper();
            m_firstFrame = true;
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            const int LeftThird = UIHelper.ScreenWidth / 6;
            const int RightThird = 4 * UIHelper.ScreenWidth / 6;

            if (m_enabled)
            {
                screen.clear();

                if (m_firstFrame)
                {
                    TCODConsole.resetCredits();
                    m_firstFrame = false;
                }

                if (!m_creditsDone)
                {
                    m_dialogColorHelper.SaveColors(screen);
                    m_creditsDone = TCODConsole.renderCredits(RightThird + 10, 54, true);
                    m_dialogColorHelper.ResetColors(screen);
                }

                screen.printFrame(0, 0, UIHelper.ScreenWidth, UIHelper.ScreenHeight, false, TCODBackgroundFlag.Set, "Help");

                screen.printRect(RightThird - 19, 52, 45, 7, "Copyright Chris Hamons 2010.\nThank you Ben for\nearly development help.\n\nSoli Deo Gloria");

                const int SymbolStartY = 3;
                const int SymbolVerticalOffset = -8;
                screen.print(LeftThird + SymbolVerticalOffset, SymbolStartY, "Map Symbols");
                screen.print(LeftThird + SymbolVerticalOffset, SymbolStartY + 1, "------------------------");
                screen.print(LeftThird + SymbolVerticalOffset, SymbolStartY + 2, "@ - You");
                screen.print(LeftThird + SymbolVerticalOffset, SymbolStartY + 3, "; - Open Door");
                screen.print(LeftThird + SymbolVerticalOffset, SymbolStartY + 4, ": - Closed Door");
                screen.print(LeftThird + SymbolVerticalOffset, SymbolStartY + 5, "_ - Cosmetic");
                screen.print(LeftThird + SymbolVerticalOffset, SymbolStartY + 6, "& - Item on Ground");
                screen.print(LeftThird + SymbolVerticalOffset, SymbolStartY + 7, "%% - Stack of Items");

                const int ActionStartY = SymbolStartY + 10;
                const int ActionVerticalOffset = -8;
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY, "Action Keys");
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 1, "------------------------");
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 2, "Attack (or reload) - " + m_keyMappings["Attack"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 3, "Get Item           - " + m_keyMappings["GetItem"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 4, "Cast Spell         - " + m_keyMappings["CastSpell"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 5, "Inventory          - " + m_keyMappings["Inventory"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 6, "Equipment          - " + m_keyMappings["Equipment"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 7, "Skill Tree         - " + m_keyMappings["ShowSkillTree"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 8, "Operate            - " + m_keyMappings["Operate"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 9, "Wait               - " + m_keyMappings["Wait"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 10, "Rest Until Healed  - " + m_keyMappings["RestTillHealed"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 11, "Move To Location   - " + m_keyMappings["MoveToLocation"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 12, "View Mode          - " + m_keyMappings["ViewMode"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 13, "Down Stairs        - " + m_keyMappings["DownStairs"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 14, "Up Stairs          - " + m_keyMappings["UpStairs"]);
                screen.print(LeftThird + ActionVerticalOffset, ActionStartY + 15, "Swap Weapons       - " + m_keyMappings["SwapWeapon"]);

                const int RebindingStartY = SymbolStartY;
                string rebindingText = "To change keystroke bindings, edit KeyMappings.xml and restart magecrawl.\n\nA list of possible keys can be found at: http://tinyurl.com/ya5l8sj";
                screen.printFrame(RightThird - 12, RebindingStartY, 31, 9, true);
                screen.printRect(RightThird - 11, RebindingStartY + 1, 29, 8, rebindingText);

                const int DirectionStartY = ActionStartY + 18;
                const int DirectionVerticalOffset = -8;
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY, string.Format("Direction Keys (+{0} to Run)", (bool)Preferences.Instance["UseAltInsteadOfCtrlForRunning"] ? "Alt" : "Ctrl"));
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 1, "------------------------");
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 2, "North      - " + m_keyMappings["North"]);
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 3, "West       - " + m_keyMappings["West"]);
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 4, "East       - " + m_keyMappings["East"]);
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 5, "South      - " + m_keyMappings["South"]);
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 6, "North West - " + m_keyMappings["Northwest"]);
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 7, "North East - " + m_keyMappings["Northeast"]);
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 8, "South West - " + m_keyMappings["Southwest"]);
                screen.print(LeftThird + DirectionVerticalOffset, DirectionStartY + 9, "South East - " + m_keyMappings["Southeast"]);

                const int OtherKeysStartY = ActionStartY;
                const int OtherKeysVerticalOffset = -12;
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY, "Other Keys");
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 1, "----------------------------");
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 2, "Text Box Page Up   - " + m_keyMappings["TextBoxPageUp"]);
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 3, "Text Box Page Down - " + m_keyMappings["TextBoxPageDown"]);
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 4, "Text Box Clear     - " + m_keyMappings["TextBoxClear"]);
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 5, "Escape             - " + m_keyMappings["Escape"]);
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 6, "Select             - " + m_keyMappings["Select"]);
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 7, "Save               - " + m_keyMappings["Save"]);
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 8, "Help               - " + m_keyMappings["Help"]);
                screen.print(RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 9, "Quit               - " + m_keyMappings["Quit"]);
            }
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {         
        }

        public override void Dispose()
        {
        }

        internal void Enable(Dictionary<string, string> keyMappings)
        {
            m_enabled = true;
            m_keyMappings = keyMappings;
            m_creditsDone = false;
            m_firstFrame = true;
        }

        internal void Disable()
        {
            m_enabled = false;
        }
    }
}
