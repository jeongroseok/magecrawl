using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.ListSelection;
using Magecrawl.GameUI.ListSelection.Requests;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Keyboard.Requests;
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

        public override void NowPrimaried(object request)
        {
            m_keystroke = (NamedKey)request;
            m_gameInstance.SendPaintersRequest(new DisableAllOverlays());
            ListItemShouldBeEnabled magicSpellEnabledDelegate = s => m_engine.PlayerCouldCastSpell((ISpell)s);
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, m_engine.Player.Spells.OfType<INamedItem>().ToList(), "Spellbook", magicSpellEnabledDelegate));
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
                m_gameInstance.SendPaintersRequest(new ListSelectionItemSelectedByChar(keystroke.Character, new ListItemSelected(SpellSelectedDelegate)));
            }
        }

        private void SpellSelectedDelegate(INamedItem spellName)
        {
            ISpell spell = (ISpell)spellName;
            if (!m_engine.PlayerCouldCastSpell(spell))
                return;

            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false));           

            if (spell.TargetType.StartsWith("Single Range"))
            {
                List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListFromBurstPosition(m_engine.Player.Position, spell.Range);
                m_engine.FilterNotTargetablePointsFromList(targetablePoints, true);
                m_engine.FilterNotVisibleBothWaysFromList(targetablePoints);                

                OnTargetSelection selectionDelegate = new OnTargetSelection(s =>
                {
                    if (s != m_engine.Player.Position)
                        m_engine.PlayerCastSpell(spell, s);
                    return false;
                });
                m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetablePoints, selectionDelegate, m_keystroke, TargettingKeystrokeHandler.TargettingType.Monster));
            }
            else if (spell.TargetType == "Self")
            {
                m_engine.PlayerCastSpell(spell, m_engine.Player.Position);
                m_gameInstance.ResetHandlerName();
                m_gameInstance.UpdatePainters();
            }
            else
            {
                throw new System.ArgumentException("Don't know how to cast things with target type: " + spell.TargetType);
            }
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(new ListItemSelected(SpellSelectedDelegate)));          
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void HandleDirection(Direction direction)
        {
            m_gameInstance.SendPaintersRequest(new ChangeListSelectionPosition(direction));
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
