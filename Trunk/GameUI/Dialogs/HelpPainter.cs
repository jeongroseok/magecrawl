using System.Collections.Generic;
using System.Linq;
using libtcodWrapper;
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

        public override void DrawNewFrame(Console screen)
        {
            const int LeftThird = UIHelper.ScreenWidth / 6;
            const int RightThird = 4 * UIHelper.ScreenWidth / 6;

            if (m_enabled)
            {
                screen.Clear();

                if (m_firstFrame)
                {
                    screen.ResetCreditsAnimation();
                    m_firstFrame = false;
                }

                if (!m_creditsDone)
                {
                    m_dialogColorHelper.SaveColors(screen);
                    m_creditsDone = screen.ConsoleCreditsRender(RightThird + 10, 54, true);
                    m_dialogColorHelper.ResetColors(screen);
                }

                screen.DrawFrame(0, 0, UIHelper.ScreenWidth, UIHelper.ScreenHeight, false, "Help");

                string thanksText = "Copyright Chris Hamons 2010.\nThank you Ben for\nearly development help.\n\nSoli Deo Gloria";
                screen.PrintLineRect(thanksText, RightThird - 19, 52, 45, 7, LineAlignment.Left);

                const int SymbolStartY = 3;
                const int SymbolVerticalOffset = -8;
                screen.PrintLine("Map Symbols", LeftThird + SymbolVerticalOffset, SymbolStartY, LineAlignment.Left);
                screen.PrintLine("------------------------", LeftThird + SymbolVerticalOffset, SymbolStartY + 1, LineAlignment.Left);
                screen.PrintLine("@ - You", LeftThird + SymbolVerticalOffset, SymbolStartY + 2, LineAlignment.Left);
                screen.PrintLine("; - Open Door", LeftThird + SymbolVerticalOffset, SymbolStartY + 3, LineAlignment.Left);
                screen.PrintLine(": - Closed Door", LeftThird + SymbolVerticalOffset, SymbolStartY + 4, LineAlignment.Left);
                screen.PrintLine("_ - Cosmetic", LeftThird + SymbolVerticalOffset, SymbolStartY + 5, LineAlignment.Left);
                screen.PrintLine("& - Item on Ground", LeftThird + SymbolVerticalOffset, SymbolStartY + 6, LineAlignment.Left);
                screen.PrintLine("%% - Stack of Items", LeftThird + SymbolVerticalOffset, SymbolStartY + 7, LineAlignment.Left);

                const int ActionStartY = SymbolStartY + 10;
                const int ActionVerticalOffset = -8;
                screen.PrintLine("Action Keys", LeftThird + ActionVerticalOffset, ActionStartY, LineAlignment.Left);
                screen.PrintLine("------------------------", LeftThird + ActionVerticalOffset, ActionStartY + 1, LineAlignment.Left);
                screen.PrintLine("Attack (or reload) - " + m_keyMappings["Attack"], LeftThird + ActionVerticalOffset, ActionStartY + 2, LineAlignment.Left);
                screen.PrintLine("Get Item           - " + m_keyMappings["GetItem"], LeftThird + ActionVerticalOffset, ActionStartY + 3, LineAlignment.Left);
                screen.PrintLine("Cast Spell         - " + m_keyMappings["CastSpell"], LeftThird + ActionVerticalOffset, ActionStartY + 4, LineAlignment.Left);
                screen.PrintLine("Inventory          - " + m_keyMappings["Inventory"], LeftThird + ActionVerticalOffset, ActionStartY + 5, LineAlignment.Left);
                screen.PrintLine("Equipment          - " + m_keyMappings["Equipment"], LeftThird + ActionVerticalOffset, ActionStartY + 6, LineAlignment.Left);
                screen.PrintLine("Operate            - " + m_keyMappings["Operate"], LeftThird + ActionVerticalOffset, ActionStartY + 7, LineAlignment.Left);
                screen.PrintLine("Wait               - " + m_keyMappings["Wait"], LeftThird + ActionVerticalOffset, ActionStartY + 8, LineAlignment.Left);
                screen.PrintLine("Move To Location   - " + m_keyMappings["MoveToLocation"], LeftThird + ActionVerticalOffset, ActionStartY + 9, LineAlignment.Left);
                screen.PrintLine("View Mode          - " + m_keyMappings["ViewMode"], LeftThird + ActionVerticalOffset, ActionStartY + 10, LineAlignment.Left);
                screen.PrintLine("Down Stairs        - " + m_keyMappings["DownStairs"], LeftThird + ActionVerticalOffset, ActionStartY + 11, LineAlignment.Left);
                screen.PrintLine("Up Stairs          - " + m_keyMappings["UpStairs"], LeftThird + ActionVerticalOffset, ActionStartY + 12, LineAlignment.Left);
                screen.PrintLine("Swap Weapons       - " + m_keyMappings["SwapWeapon"], LeftThird + ActionVerticalOffset, ActionStartY + 13, LineAlignment.Left);

                const int RebindingStartY = SymbolStartY;
                string rebindingText = "To change keystroke bindings, edit KeyMappings.xml and restart magecrawl.\n\nA list of possible keys can be found at: http://tinyurl.com/ya5l8sj";
                screen.DrawFrame(RightThird - 12, RebindingStartY, 31, 9, true);
                screen.PrintLineRect(rebindingText, RightThird - 11, RebindingStartY + 1, 29, 8, LineAlignment.Left);

                const int DirectionStartY = ActionStartY + 16;
                const int DirectionVerticalOffset = -8;
                screen.PrintLine("Direction Keys (+Ctrl to Run)", LeftThird + DirectionVerticalOffset, DirectionStartY, LineAlignment.Left);
                screen.PrintLine("------------------------", LeftThird + DirectionVerticalOffset, DirectionStartY + 1, LineAlignment.Left);
                screen.PrintLine("North      - " + m_keyMappings["North"], LeftThird + DirectionVerticalOffset, DirectionStartY + 2, LineAlignment.Left);
                screen.PrintLine("West       - " + m_keyMappings["West"], LeftThird + DirectionVerticalOffset, DirectionStartY + 3, LineAlignment.Left);
                screen.PrintLine("East       - " + m_keyMappings["East"], LeftThird + DirectionVerticalOffset, DirectionStartY + 4, LineAlignment.Left);
                screen.PrintLine("South      - " + m_keyMappings["South"], LeftThird + DirectionVerticalOffset, DirectionStartY + 5, LineAlignment.Left);
                screen.PrintLine("North West - " + m_keyMappings["Northwest"], LeftThird + DirectionVerticalOffset, DirectionStartY + 6, LineAlignment.Left);
                screen.PrintLine("North East - " + m_keyMappings["Northeast"], LeftThird + DirectionVerticalOffset, DirectionStartY + 7, LineAlignment.Left);
                screen.PrintLine("South West - " + m_keyMappings["Southwest"], LeftThird + DirectionVerticalOffset, DirectionStartY + 8, LineAlignment.Left);
                screen.PrintLine("South East - " + m_keyMappings["Southeast"], LeftThird + DirectionVerticalOffset, DirectionStartY + 9, LineAlignment.Left);

                const int OtherKeysStartY = ActionStartY;
                const int OtherKeysVerticalOffset = -12;
                screen.PrintLine("Other Keys", RightThird + OtherKeysVerticalOffset, OtherKeysStartY, LineAlignment.Left);
                screen.PrintLine("----------------------------", RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 1, LineAlignment.Left);
                screen.PrintLine("Text Box Page Up   - " + m_keyMappings["TextBoxPageUp"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 2, LineAlignment.Left);
                screen.PrintLine("Text Box Page Down - " + m_keyMappings["TextBoxPageDown"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 3, LineAlignment.Left);
                screen.PrintLine("Text Box Clear     - " + m_keyMappings["TextBoxClear"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 4, LineAlignment.Left);
                screen.PrintLine("Escape             - " + m_keyMappings["Escape"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 5, LineAlignment.Left);
                screen.PrintLine("Select             - " + m_keyMappings["Select"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 6, LineAlignment.Left);
                screen.PrintLine("Save               - " + m_keyMappings["Save"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 7, LineAlignment.Left);
                screen.PrintLine("Help               - " + m_keyMappings["Help"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 8, LineAlignment.Left);
                screen.PrintLine("Quit               - " + m_keyMappings["Quit"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 9, LineAlignment.Left);
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
