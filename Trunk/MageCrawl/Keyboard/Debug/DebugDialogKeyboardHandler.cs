using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.ListSelection.Requests;
using Magecrawl.GameUI.Map.Requests;

namespace Magecrawl.Keyboard.Debug
{
    internal class DebugDialogKeyboardHandler : BaseKeystrokeHandler
    {
        private class TextElement : INamedItem
        {
            private string m_name;

            public TextElement(string name)
            {
                m_name = name;
            }

            public string DisplayName
            {
                get
                {
                    return m_name;
                }
            }
        }

        private enum OptionMode 
        {
            Options,
            CreateItem,
            CreateMonster,
            MapDebugging 
        }

        private OptionMode m_option;

        public DebugDialogKeyboardHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_option = OptionMode.Options;
        }

        public override void NowPrimaried(object request)
        {       
            SetDebugMenu();
            m_gameInstance.UpdatePainters();
        }

        private void SetDebugMenu()
        {
            m_option = OptionMode.Options;
            List<INamedItem> itemList = new List<INamedItem>() { new TextElement("Create Item"), new TextElement("Create Monster"), new TextElement("Map Debug Settings"), new TextElement("Exit") };
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, itemList, false, "Debug Options"));
        }

        private void SetCreateItemMenu()
        {
            m_option = OptionMode.CreateItem;
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, (List<INamedItem>)m_engine.DebugRequest("GetAllItemList", null), false, "Item To Spawn"));
        }

        private void SetMonsterMenu()
        {
            m_option = OptionMode.CreateMonster;
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, (List<INamedItem>)m_engine.DebugRequest("GetAllMonsterList", null), false, "Item To Spawn"));
        }

        private void CreateMapDebugSettings()
        {
            m_option = OptionMode.MapDebugging;
            List<INamedItem> mapSettings = new List<INamedItem>() { new TextElement("Debug Map Moveable"), new TextElement("Debug FOV"), new TextElement("Toggle FOV") };
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, mapSettings, false, "Map Debug Settings"));
        }

        private void OptionSelectedDelegate(INamedItem item)
        {
            if (item == null)
                return;

            TextElement element = (TextElement)item;
            switch (element.DisplayName)
            {
                case "Create Item":
                {
                    SetCreateItemMenu();
                    m_gameInstance.UpdatePainters();
                    return;
                }
                case "Create Monster":
                {
                    SetMonsterMenu();
                    m_gameInstance.UpdatePainters();
                    return;
                }
                case "Map Debug Settings":
                {
                    CreateMapDebugSettings();
                    m_gameInstance.UpdatePainters();
                    return;
                }
                case "Exit":
                default:
                {
                    Escape();
                    return;
                }
            }
        }

        private void CreateItemSelectedDelegate(INamedItem item)
        {
            if (item == null)
                return;
            m_engine.DebugRequest("SpawnItem", item.DisplayName);
            m_option = OptionMode.Options;
            Escape();
        }

        private void CreateMonsterSelectedDelegate(INamedItem item)
        {
            if (item == null)
                return;
            m_engine.DebugRequest("SpawnMonster", item.DisplayName);
            m_option = OptionMode.Options;
            Escape();
        }

        private void MapDebuggingSelectedDelegate(INamedItem item)
        {
            if (item == null)
                return;

            switch (item.DisplayName)
            {
                case "Debug Map Moveable":
                    m_gameInstance.SendPaintersRequest(new ToggleDebuggingMoveable(m_engine));
                    m_gameInstance.UpdatePainters();
                    break;
                case "Debug FOV":
                    m_gameInstance.SendPaintersRequest(new ToggleDebuggingFOV(m_engine));
                    m_gameInstance.UpdatePainters();
                    break;
                case "Toggle FOV":
                    m_gameInstance.SendPaintersRequest(new SwapFOVEnabledStatus());
                    m_gameInstance.UpdatePainters();
                    break;
            }
            m_option = OptionMode.Options;
            Escape();
        }   

        private void Escape()
        {
            if (m_option == OptionMode.Options)
            {
                m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(false));
                m_gameInstance.UpdatePainters();
                m_gameInstance.ResetHandlerName();
            }
            else
            {
                SetDebugMenu();
            }
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

        private void Select()
        {
            switch (m_option)
            {
                case OptionMode.Options:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(OptionSelectedDelegate));
                    return;
                case OptionMode.CreateItem:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(CreateItemSelectedDelegate));
                    return;
                case OptionMode.MapDebugging:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(MapDebuggingSelectedDelegate));
                    return;
                case OptionMode.CreateMonster:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(CreateMonsterSelectedDelegate));
                    return;
            }            
        }
    }
}
