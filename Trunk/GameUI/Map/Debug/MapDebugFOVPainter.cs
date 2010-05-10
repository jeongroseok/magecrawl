using System.Collections.Generic;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Debug
{
    internal sealed class MapDebugFOVPainter : MapPainterBase
    {
        private bool m_enabled;
        private Point m_mapUpCorner;
        private List<Point> m_playerFOV;
        private Dictionary<ICharacter, List<Point>> m_monsterFOV;
        private Dictionary<int, TCODColor> m_monsterFOVColor;

        public MapDebugFOVPainter()
        {
            m_enabled = false;
            m_playerFOV = null;
            m_monsterFOV = null;
            m_mapUpCorner = new Point();
            m_monsterFOVColor = new Dictionary<int, TCODColor>();
        }

        private TCODColor GetColorForMonster(ICharacter character)
        {
            if (!m_monsterFOVColor.ContainsKey(character.UniqueID))
            {
                m_monsterFOVColor[character.UniqueID] = ColorGenerator.GenerateRandomColor(false);
            }
            return m_monsterFOVColor[character.UniqueID];
        }

        public void SwapDebugFOV(IGameEngine engine)
        {
            m_enabled = !m_enabled;
            CalculateFOV(engine);
        }

        public override void UpdateFromNewData(IGameEngine engine, Point mapUpCorner, Point cursorPosition)
        {
            CalculateFOV(engine);

            m_mapUpCorner = mapUpCorner;
        }

        private void CalculateFOV(IGameEngine engine)
        {
            // This is expensive, so only do if we've going to use it
            if (m_enabled)
            {
                m_playerFOV = engine.CellsInPlayersFOV();
                m_monsterFOV = engine.CellsInAllMonstersFOV();
            }
        }

        public override void DrawNewFrame(TCODConsole screen)
        {
            if (m_enabled)
            {
                foreach (Point p in m_playerFOV)
                {
                    int screenPlacementX = m_mapUpCorner.X + p.X + 1;
                    int screenPlacementY = m_mapUpCorner.Y + p.Y + 1;
                    if (IsDrawableTile(screenPlacementX, screenPlacementY))
                        screen.setCharBackground(screenPlacementX, screenPlacementY, ColorPresets.DarkRed);
                }
                foreach (ICharacter c in m_monsterFOV.Keys)
                {
                    foreach (Point p in m_monsterFOV[c])
                    {
                        int screenPlacementX = m_mapUpCorner.X + p.X + 1;
                        int screenPlacementY = m_mapUpCorner.Y + p.Y + 1;
                        if (IsDrawableTile(screenPlacementX, screenPlacementY))
                        {
                            TCODColor currentColor = screen.getCharBackground(screenPlacementX, screenPlacementY);
                            screen.setCharBackground(screenPlacementX, screenPlacementY, TCODColor.Interpolate(currentColor, GetColorForMonster(c), .6f));
                        }
                    }
                }
            }
        }

        internal override void DisableAllOverlays()
        {
            m_enabled = false;
        }
    }
}