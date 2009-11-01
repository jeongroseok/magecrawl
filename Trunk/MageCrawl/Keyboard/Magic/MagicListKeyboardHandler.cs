using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard.Magic
{
    internal class MagicListKeyboardHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        // When we're brought up, get the keystroke used to call us, so we can use it to select target(s)
        private NamedKey m_keystroke;

        public MagicListKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            m_keystroke = (NamedKey)objOne;
            m_gameInstance.SendPaintersRequest("DisableAllOverlays");
            m_gameInstance.SendPaintersRequest("ShowListSelectionWindow", m_engine.Player.Spells.OfType<INamedItem>().ToList(), "Spellbook");
            m_gameInstance.UpdatePainters();
        }

        public override void HandleKeystroke(NamedKey keystroke)
        {
            MethodInfo action;
            m_keyMappings.TryGetValue(keystroke, out action);
            if (action != null)
            {
                action.Invoke(this, null);
            }
            else if (keystroke.Code == libtcodWrapper.KeyCode.TCODK_CHAR)
            {
                m_gameInstance.SendPaintersRequest("ListSelectionItemSelectedByChar", new Magecrawl.GameUI.ListSelection.ListItemSelected(SpellSelectedDelegate), keystroke.Character);
            }
        }

        private void SpellSelectedDelegate(INamedItem spellName)
        {
            m_gameInstance.SendPaintersRequest("StopListSelectionWindow");
            m_gameInstance.ResetHandlerName();

            ISpell spell = (ISpell)spellName;
            if (m_engine.PlayerCouldCastSpell(spell))
            {
                if (spell.TargetType != "None")
                {
                    string[] targetParts = spell.TargetType.Split(':');
                    int range = int.Parse(targetParts[1]);
                    List<EffectivePoint> targetablePoints = PointListUtils.PointListFromBurstPosition(m_engine.Player.Position, range);
                    m_engine.FilterNotTargetablePointsFromList(targetablePoints);
                    OnTargetSelection selectionDelegate = new OnTargetSelection(s =>
                    {
                        m_engine.PlayerCastSpell(spell, s);
                        m_gameInstance.UpdatePainters();
                    });
                    m_gameInstance.SetHandlerName("Target", targetablePoints, selectionDelegate, m_keystroke);
                }
                else
                {
                    m_engine.PlayerCastSpell(spell, Point.Invalid);
                    m_gameInstance.UpdatePainters();
                }
            }
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest("ListSelectionItemSelected", new Magecrawl.GameUI.ListSelection.ListItemSelected(SpellSelectedDelegate));          
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest("StopListSelectionWindow");
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void HandleDirection(Direction direction)
        {
            m_gameInstance.SendPaintersRequest("ListSelectionPositionChanged", direction);
            m_gameInstance.UpdatePainters();
        }

        private void North()
        {
            HandleDirection(Direction.North);
        }

        private void South()
        {
            HandleDirection(Direction.South);
        }
    }
}
