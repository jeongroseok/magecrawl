using System.Collections.Generic;
using System.Linq;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.Items;

namespace Magecrawl.GameEngine
{
    internal class DebugEngine : IDebugger
    {
        private CoreGameEngine m_engine;

        internal DebugEngine(CoreGameEngine engine)
        {
            m_engine = engine;
        }

        // For the IsPathable debugging mode, show if player could walk there.
        public bool[,] PlayerMoveableToEveryPoint()
        {
            return m_engine.PlayerMoveableToEveryPoint();
        }

        public Dictionary<ICharacter, List<Point>> CellsInAllMonstersFOV()
        {
            Dictionary<ICharacter, List<Point>> returnValue = new Dictionary<ICharacter, List<Point>>();

            foreach (ICharacter c in m_engine.Map.Monsters)
            {
                returnValue[c] = GenerateFOVListForCharacter(c);
            }

            return returnValue;
        }

        public List<Point> CellsInPlayersFOV()
        {
            return GenerateFOVListForCharacter(m_engine.Player);
        }

        // Shared with DebugEngine
        private List<Point> GenerateFOVListForCharacter(ICharacter c)
        {
            List<Point> returnList = new List<Point>();

            m_engine.FOVManager.CalculateForMultipleCalls(m_engine.Map, c.Position, c.Vision);

            for (int i = 0; i < m_engine.Map.Width; ++i)
            {
                for (int j = 0; j < m_engine.Map.Height; ++j)
                {
                    Point currentPosition = new Point(i, j);
                    if (m_engine.FOVManager.Visible(currentPosition))
                    {
                        returnList.Add(currentPosition);
                    }
                }
            }
            return returnList;
        }

        // This is a catch all debug request interface, used for debug menus.
        // While I could provide a nice interface typesafe and all for all requests,
        // this is easier to do. What was that about the cobbler's children again?
        public object DebugRequest(string request, object argument)
        {
            switch (request)
            {
                case "GetAllItemList":
                    List<INamedItem> allItemList = new List<INamedItem>();
                    foreach (string s in m_engine.ItemFactory.ItemTypeList)
                        allItemList.Add(new TextElement(s));
                    return allItemList;                    
                case "SpawnItem":
                {
                    string itemName = ((Pair<string, int>)argument).First;
                    int level = ((Pair<string, int>)argument).Second;
                    Item itemCreated = m_engine.ItemFactory.CreateItemOfType(itemName, level);
                    m_engine.Map.AddItem(new Pair<Item, Point>(itemCreated, m_engine.Player.Position));
                    return null;
                }
                case "GetAllMonsterList":
                    return m_engine.MonsterFactory.GetAllMonsterListForDebug().OfType<INamedItem>().ToList();
                case "SpawnMonster":
                    {
                        Point playerPos = m_engine.Player.Position;
                        for (int i = -1; i <= 1; ++i)
                        {
                            for (int j = -1; j <= 1; ++j)
                            {
                                if (i == 0 && j == 0)
                                    continue;
                                Point newPosition = playerPos + new Point(i, j);
                                if (m_engine.Map.IsPointOnMap(newPosition))
                                {
                                    if (m_engine.PathToPoint(m_engine.Player, newPosition, false, true, false) != null &&
                                        m_engine.Map.Monsters.Where(m => m.Position == newPosition).Count() == 0)
                                    {
                                        m_engine.Map.AddMonster(m_engine.MonsterFactory.CreateMonster((string)argument, newPosition));
                                        return null;
                                    }
                                }
                            }
                        }
                        return null;
                    }
                case "AddSkillPoints":
                    m_engine.Player.SkillPoints += (int)argument;
                    return null;
                case "KillMonstersOnFloor":
                    m_engine.Map.ClearMonstersFromMap();
                    return null;
                default:
                    return null;
            }
        }
    }
}
