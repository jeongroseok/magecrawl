using System.Collections.Generic;
using System.Linq;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.GameEngine.Skills;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Magic;

namespace Magecrawl.GameEngine
{
    internal class GameStateInterface : IGameState
    {
        private CoreGameEngine m_engine;

        public GameStateInterface(CoreGameEngine engine)
        {
            m_engine = engine;
        }

        public TileVisibility[,] CalculateTileVisibility()
        {
            return m_engine.CalculateTileVisibility();
        }       

        public bool DangerInLOS()
        {
            return m_engine.DangerPlayerInLOS();
        }

        public bool CurrentOrRecentDanger()
        {
            return m_engine.CurrentOrRecentDanger();
        }

        public List<ICharacter> MonstersInPlayerLOS()
        {
            return m_engine.MonstersInPlayerLOS();
        }

        public List<string> GetDescriptionForTile(Point p)
        {
            if (!m_engine.FOVManager.VisibleSingleShot(m_engine.Map, m_engine.Player.Position, m_engine.Player.Vision, p))
                return new List<string>();

            List<string> descriptionList = new List<string>();
            if (m_engine.Player.Position == p)
                descriptionList.Add(m_engine.Player.Name);

            ICharacter monsterAtLocation = m_engine.Map.Monsters.Where(monster => monster.Position == p).FirstOrDefault();
            if (monsterAtLocation != null)
                descriptionList.Add(monsterAtLocation.Name);

            IMapObject mapObjectAtLocation = m_engine.Map.MapObjects.Where(mapObject => mapObject.Position == p).FirstOrDefault();
            if (mapObjectAtLocation != null)
                descriptionList.Add(mapObjectAtLocation.Name);

            foreach (Pair<IItem, Point> i in m_engine.Map.Items.Where(i => i.Second == p))
                descriptionList.Add(i.First.DisplayName);

            if (descriptionList.Count == 0)
                descriptionList.Add(m_engine.Map.GetTerrainAt(p).ToString());
            return descriptionList;
        }

        public ISkill GetSkillFromName(string name)
        {
            return SkillFactory.Instance.CreateSkill(name);
        }

        public StairMovmentType IsStairMovementSpecial(bool headingUp)
        {
            Stairs s = m_engine.Map.MapObjects.OfType<Stairs>().Where(x => x.Position == m_engine.Player.Position).SingleOrDefault();
            if (s != null)
            {
                if (s.Type == MapObjectType.StairsUp && m_engine.CurrentLevel == 0 && headingUp)
                    return StairMovmentType.QuitGame;
                else if (s.Type == MapObjectType.StairsDown && m_engine.CurrentLevel == (m_engine.NumberOfLevels - 1) && !headingUp)
                    return StairMovmentType.WinGame;
            }
            return StairMovmentType.None;
        }

        public List<ItemOptions> GetOptionsForInventoryItem(IItem item)
        {
            return m_engine.GetOptionsForInventoryItem(item as Item);
        }

        public List<ItemOptions> GetOptionsForEquipmentItem(IItem item)
        {
            return m_engine.GetOptionsForEquipmentItem(item as Item);
        }

        public List<EffectivePoint> CalculateTargetablePointsForEquippedWeapon()
        {
            return CombatEngine.CalculateTargetablePointsForEquippedWeapon((Weapon)m_engine.Player.CurrentWeapon, m_engine.Player.Position, m_engine.Player.Vision);
        }

        public string GetSpellSchoolForItem(IItem item)
        {
            if (((Item)item).ContainsAttribute("InvokeSpellEffect"))
            {
                string effectName = ((Item)item).GetAttribute("InvokeSpellEffect");
                return SpellFactory.Instance.CreateSpell(effectName).School;
            }
            else
            {
                return null;
            }
        }
    }
}
