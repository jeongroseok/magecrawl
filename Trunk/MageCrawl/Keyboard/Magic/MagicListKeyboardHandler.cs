using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.ListSelection;
using Magecrawl.GameUI.ListSelection.Requests;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard.Magic
{
    internal class MagicListKeyboardHandler : BaseKeystrokeHandler
    {
        private class SpellAnimationHelper
        {
            private Point m_point;
            private ISpell m_spell;
            private IGameEngine m_engine;
            private GameInstance m_gameInstance;

            internal SpellAnimationHelper(Point point, ISpell spell, IGameEngine engine, GameInstance gameInstance)
            {
                m_point = point;
                m_spell = spell;
                m_engine = engine;
                m_gameInstance = gameInstance;
            }

            internal void Invoke()
            {
                m_engine.PlayerCastSpell(m_spell, m_point);
                m_gameInstance.ResetHandlerName();
                m_gameInstance.UpdatePainters();
            }
        }

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
            m_gameInstance.SendPaintersRequest(new DisableAllOverlays());
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, m_engine.Player.Spells.OfType<INamedItem>().ToList(), "Spellbook"));
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
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false));

            ISpell spell = (ISpell)spellName;
            Color color = GetColorOfSpellFromSchool(spell);

            if (m_engine.PlayerCouldCastSpell(spell))
            {
                if (spell.TargetType.StartsWith("Single Range"))
                {
                    // Get targetable points.
                    string[] targetParts = spell.TargetType.Split(':');
                    int range = int.Parse(targetParts[1]);
                    List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListFromBurstPosition(m_engine.Player.Position, range);
                    m_engine.FilterNotTargetablePointsFromList(targetablePoints, true);

                    // Setup delegate to do action on target
                    OnTargetSelection selectionDelegate = new OnTargetSelection(s =>
                    {
                        // Since we want animation to go first, setup helper to run that
                        SpellAnimationHelper rangedHelper = new SpellAnimationHelper(s, spell, m_engine, m_gameInstance);
                        List<Point> pathToTarget = m_engine.PlayerPathToPoint(s);
                        EffectDone onEffectDone = new EffectDone(rangedHelper.Invoke);

                        m_gameInstance.SetHandlerName("Effects", new ShowRangedBolt(onEffectDone, pathToTarget, color));
                        m_gameInstance.UpdatePainters();
                        return true;
                    });
                    m_gameInstance.SetHandlerName("Target", targetablePoints, selectionDelegate, m_keystroke);
                }
                else if (spell.TargetType == "Self")
                {
                    SpellAnimationHelper spellHelper = new SpellAnimationHelper(m_engine.Player.Position, spell, m_engine, m_gameInstance);
                    EffectDone onEffectDone = new EffectDone(spellHelper.Invoke);
                    m_gameInstance.SetHandlerName("Effects", new ShowSelfBuff(onEffectDone, m_engine.Player.Position, color));
                    m_gameInstance.UpdatePainters();
                }
                else
                {
                    throw new System.ArgumentException("Don't know how to cast things with target type: " + spell.TargetType);
                }
            }
        }

        private Color GetColorOfSpellFromSchool(ISpell spell)
        {
            switch (spell.School)
            {
                case "Light":
                    return ColorPresets.Wheat;
                case "Darkness":
                    return ColorPresets.DarkGray;
                case "Fire":
                    return ColorPresets.Firebrick;
                case "Arcane":
                    return ColorPresets.DarkViolet;
                case "Air":
                    return ColorPresets.LightBlue;
                case "Earth":
                    return ColorPresets.SaddleBrown;
                default:
                    return TCODColorPresets.White;
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
