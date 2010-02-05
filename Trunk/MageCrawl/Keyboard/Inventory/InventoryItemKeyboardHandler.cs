using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Inventory.Requests;
using Magecrawl.Utilities;
using Magecrawl.Keyboard.Requests;

namespace Magecrawl.Keyboard.Inventory
{
    internal sealed class InventoryItemKeyboardHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;
        private string m_handlerWhoCalledMe;

        public InventoryItemKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_handlerWhoCalledMe = String.Empty;
        }

        public override void NowPrimaried(object request)
        {
            m_gameInstance.UpdatePainters();
            m_handlerWhoCalledMe = (string)request;
        }

        private void Select()
        {
            m_gameInstance.SendPaintersRequest(new SelectInventoryItemOption(new Magecrawl.GameUI.Inventory.InventoryItemOptionSelected(InventoryItemOptionSelectedDelegate)));
        }

        private void InventoryItemOptionSelectedDelegate(IItem item, string optionName)
        {
            if (item == null)
            {
                Escape();
                return;
            }

            string targettingNeeded = m_engine.GetTargettingTypeForInventoryItem(item, optionName);
            
            if (targettingNeeded == null)
            {
                InvokeSelected(item, optionName, null);
            }
            else if (targettingNeeded == "Self")
            {
                InvokeSelected(item, optionName, m_engine.Player.Position);
            }
            else if (targettingNeeded.StartsWith("Single Range"))
            {
                m_gameInstance.SendPaintersRequest(new ShowInventoryItemWindow(false));

                int range = int.Parse(targettingNeeded.Split(':')[1]);
                List<EffectivePoint> targetablePoints = PointListUtils.EffectivePointListFromBurstPosition(m_engine.Player.Position, range);
                m_engine.FilterNotTargetablePointsFromList(targetablePoints, true);
                m_engine.FilterNotVisibleBothWaysFromList(targetablePoints);

                m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetablePoints, new OnTargetSelection(x => { InvokeSelected(item, optionName, x); return false; }),
                    NamedKey.Invalid, TargettingKeystrokeHandler.TargettingType.Monster));
            }
            else
            {
                throw new InvalidOperationException("Don't know how to do that kind of targettings");                       
            }
        }

        private void InvokeSelected(IItem item, string optionName, object arguemnt)
        {
            m_engine.PlayerSelectedItemOption(item, optionName, arguemnt);
            m_gameInstance.SendPaintersRequest(new ShowInventoryItemWindow(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new ShowInventoryItemWindow(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.SetHandlerName(m_handlerWhoCalledMe, true);   // Gets picked up in InventoryScreenKeyboardHandler::NowPrimaried
        }

        private void HandleDirection(Direction direction)
        {
            m_gameInstance.SendPaintersRequest(new ChangeInventoryItemPosition(direction));
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
