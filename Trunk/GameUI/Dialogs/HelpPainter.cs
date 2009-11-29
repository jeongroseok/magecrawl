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

        internal HelpPainter()
        {
            m_enabled = false;
        }

        public override void DrawNewFrame(Console screen)
        {
            const int LeftThird = UIHelper.ScreenWidth / 6;
            const int RightThird = 4 * UIHelper.ScreenWidth / 6;

            if (m_enabled)
            {
                screen.DrawFrame(0, 0, UIHelper.ScreenWidth, UIHelper.ScreenHeight, true, "Help");

                const int ActionStartY = 36;
                const int ActionVerticalOffset = -8;
                screen.PrintLine("Action Keys", LeftThird + ActionVerticalOffset, ActionStartY, LineAlignment.Left);
                screen.PrintLine("------------------------", LeftThird + ActionVerticalOffset, ActionStartY + 1, LineAlignment.Left);
                screen.PrintLine("Attack      - " + m_keyMappings["Attack"], LeftThird + ActionVerticalOffset, ActionStartY + 2, LineAlignment.Left);
                screen.PrintLine("Get Item    - " + m_keyMappings["GetItem"], LeftThird + ActionVerticalOffset, ActionStartY + 3, LineAlignment.Left);
                screen.PrintLine("Cast Spell  - " + m_keyMappings["CastSpell"], LeftThird + ActionVerticalOffset, ActionStartY + 4, LineAlignment.Left);
                screen.PrintLine("Inventory   - " + m_keyMappings["Inventory"], LeftThird + ActionVerticalOffset, ActionStartY + 5, LineAlignment.Left);
                screen.PrintLine("Equipment   - " + m_keyMappings["Equipment"], LeftThird + ActionVerticalOffset, ActionStartY + 6, LineAlignment.Left);
                screen.PrintLine("Operate     - " + m_keyMappings["Operate"], LeftThird + ActionVerticalOffset, ActionStartY + 7, LineAlignment.Left);
                screen.PrintLine("Wait        - " + m_keyMappings["Wait"], LeftThird + ActionVerticalOffset, ActionStartY + 8, LineAlignment.Left);
                screen.PrintLine("View Mode   - " + m_keyMappings["ViewMode"], LeftThird + ActionVerticalOffset, ActionStartY + 9, LineAlignment.Left);

                const int RebindingStartY = ActionStartY + 1;
                string rebindingText = "To change keystroke bindings, edit KeyMappings.xml and restart magecrawl.\n\nA list of possible keys can be found at: http://tinyurl.com/ya5l8sj";
                screen.DrawFrame(RightThird - 12, RebindingStartY, 31, 9, true);
                screen.PrintLineRect(rebindingText, RightThird - 11, RebindingStartY + 1, 29, 8, LineAlignment.Left);

                const int DirectionStartY = 48;
                const int DirectionVerticalOffset = -8;
                screen.PrintLine("Direction Keys", LeftThird + DirectionVerticalOffset, DirectionStartY, LineAlignment.Left);
                screen.PrintLine("------------------------", LeftThird + DirectionVerticalOffset, DirectionStartY + 1, LineAlignment.Left);
                screen.PrintLine("North      - " + m_keyMappings["North"], LeftThird + DirectionVerticalOffset, DirectionStartY + 2, LineAlignment.Left);
                screen.PrintLine("West       - " + m_keyMappings["West"], LeftThird + DirectionVerticalOffset, DirectionStartY + 3, LineAlignment.Left);
                screen.PrintLine("East       - " + m_keyMappings["East"], LeftThird + DirectionVerticalOffset, DirectionStartY + 4, LineAlignment.Left);
                screen.PrintLine("South      - " + m_keyMappings["South"], LeftThird + DirectionVerticalOffset, DirectionStartY + 5, LineAlignment.Left);
                screen.PrintLine("North West - " + m_keyMappings["Northwest"], LeftThird + DirectionVerticalOffset, DirectionStartY + 6, LineAlignment.Left);
                screen.PrintLine("North East - " + m_keyMappings["Northeast"], LeftThird + DirectionVerticalOffset, DirectionStartY + 7, LineAlignment.Left);
                screen.PrintLine("South West - " + m_keyMappings["Southwest"], LeftThird + DirectionVerticalOffset, DirectionStartY + 8, LineAlignment.Left);
                screen.PrintLine("South East - " + m_keyMappings["Southeast"], LeftThird + DirectionVerticalOffset, DirectionStartY + 9, LineAlignment.Left);

                const int OtherKeysStartY = DirectionStartY;
                const int OtherKeysVerticalOffset = -12;
                screen.PrintLine("Other Keys", RightThird + OtherKeysVerticalOffset, DirectionStartY, LineAlignment.Left);
                screen.PrintLine("----------------------------", RightThird + OtherKeysVerticalOffset, DirectionStartY + 1, LineAlignment.Left);
                screen.PrintLine("Text Box Page Up   - " + m_keyMappings["TextBoxPageUp"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 2, LineAlignment.Left);
                screen.PrintLine("Text Box Page Down - " + m_keyMappings["TextBoxPageDown"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 3, LineAlignment.Left);
                screen.PrintLine("Text Box Clear     - " + m_keyMappings["TextBoxClear"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 4, LineAlignment.Left);
                screen.PrintLine("Escape             - " + m_keyMappings["Escape"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 4, LineAlignment.Left);
                screen.PrintLine("Select             - " + m_keyMappings["Select"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 5, LineAlignment.Left);
                screen.PrintLine("Save               - " + m_keyMappings["Save"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 6, LineAlignment.Left);
                screen.PrintLine("Load               - " + m_keyMappings["Load"], RightThird + OtherKeysVerticalOffset, OtherKeysStartY + 7, LineAlignment.Left);
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
        }

        internal void Disable()
        {
            m_enabled = false;
        }
    }
}
