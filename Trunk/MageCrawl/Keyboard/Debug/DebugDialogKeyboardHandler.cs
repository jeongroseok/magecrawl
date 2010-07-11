using System.Collections.Generic;
using System.ComponentModel.Composition;
using Magecrawl.GameUI.ListSelection.Requests;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard.Debug
{
    // This code is a bit hacky. It should really be rewritten and such. However, it's only
    // debug code, so as long as it works...
    [Export(typeof(IKeystrokeHandler))]
    [ExportMetadata("RequireAllActionsMapped", "false")]
    [ExportMetadata("HandlerName", "DebugMode")]
    internal class DebugDialogKeyboardHandler : BaseKeystrokeHandler
    {
        private enum OptionMode 
        {
            DebugMainMenu,
            CreateItem,
            SelectLevel,
            CreateMonster,
            MapDebugging 
        }

        private OptionMode m_option;

        private bool m_spawnItem;
        private string m_toSpawnName;

        public DebugDialogKeyboardHandler()
        {
            m_option = OptionMode.DebugMainMenu;
        }

        public override void NowPrimaried(object request)
        {       
            SetDebugMenu();
            m_gameInstance.UpdatePainters();
        }

        private void SetDebugMenu()
        {
            m_option = OptionMode.DebugMainMenu;
            List<INamedItem> itemList = new List<INamedItem>() {new TextElement("Create Item"), new TextElement("Create Monster"), 
                new TextElement("Map Debug Settings"),  new TextElement("Kill Monsters on Floor"), new TextElement("Add Skill Points"), new TextElement("Exit") };
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, itemList, false, "Debug Options"));
        }

        private void SetCreateItemMenu()
        {
            m_option = OptionMode.CreateItem;
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, (List<INamedItem>)m_engine.Debugger.DebugRequest("GetAllItemList", null), false, "Item To Spawn"));
        }

        private void SetCreateLevelMenu(int totalLevels)
        {
            m_option = OptionMode.SelectLevel;
            List<INamedItem> mapSettings = new List<INamedItem>();
            for (int i = 0; i <= totalLevels; ++i)
                mapSettings.Add(new TextElement(i.ToString()));
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, mapSettings, false, "Level To Spawn"));
        }

        private void SetMonsterMenu()
        {
            m_option = OptionMode.CreateMonster;
            m_gameInstance.SendPaintersRequest(new ShowListSelectionWindow(true, (List<INamedItem>)m_engine.Debugger.DebugRequest("GetAllMonsterList", null), false, "Monster To Spawn"));
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
                case "Add Skill Points":
                {
                    m_engine.Debugger.DebugRequest("AddSkillPoints", 50);
                    Escape();
                    return;
                }
                case "Kill Monsters on Floor":
                {
                    m_engine.Debugger.DebugRequest("KillMonstersOnFloor", null);
                    Escape();
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
            m_spawnItem = true;
            m_toSpawnName = item.DisplayName;
            SetCreateLevelMenu(10);
            m_gameInstance.UpdatePainters();
        }

        private void CreateMonsterSelectedDelegate(INamedItem monster)
        {
            m_spawnItem = false;
            m_toSpawnName = monster.DisplayName;
            SetCreateLevelMenu(5);
            m_gameInstance.UpdatePainters();
        }

        private void CreateLevelSelectedDelegate(INamedItem item)
        {
            if (item == null)
                return;
            if (m_spawnItem)
                m_engine.Debugger.DebugRequest("SpawnItem", new Pair<string, int>(m_toSpawnName, int.Parse(item.DisplayName)));
            else
                m_engine.Debugger.DebugRequest("SpawnMonster", new Pair<string, int>(m_toSpawnName, int.Parse(item.DisplayName)));
            m_option = OptionMode.DebugMainMenu;
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
            m_option = OptionMode.DebugMainMenu;
            Escape();
        }   

        private void Escape()
        {
            if (m_option == OptionMode.DebugMainMenu)
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
                case OptionMode.DebugMainMenu:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(OptionSelectedDelegate));
                    return;
                case OptionMode.CreateItem:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(CreateItemSelectedDelegate));
                    return;
                case OptionMode.CreateMonster:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(CreateMonsterSelectedDelegate));
                    return;
                case OptionMode.SelectLevel:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(CreateLevelSelectedDelegate));
                    return;
                case OptionMode.MapDebugging:
                    m_gameInstance.SendPaintersRequest(new ListSelectionItemSelected(MapDebuggingSelectedDelegate));
                    return;
            }            
        }
    }
}
