using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.Map.Debug
{
    internal sealed class MapDebugFOVPainter : MapPainterBase
    {
        private bool m_enabled;
        private int m_width;
        private int m_height;
        private Point m_mapUpCorner;
        private List<Point> m_playerFOV;
        private Dictionary<ICharacter, List<Point>> m_monsterFOV;
        private Dictionary<int, Color> m_monsterFOVColor;

        public MapDebugFOVPainter()
        {
            m_enabled = false;
            m_playerFOV = null;
            m_monsterFOV = null;
            m_width = 0;
            m_height = 0;
            m_mapUpCorner = new Point();
            m_monsterFOVColor = new Dictionary<int, Color>();
        }

        private Color GetColorForMonster(ICharacter character)
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

            m_width = engine.Map.Width;
            m_height = engine.Map.Height;
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

        public override void DrawNewFrame(Console screen)
        {
            if (m_enabled)
            {
                foreach (Point p in m_playerFOV)
                {
                    int screenPlacementX = m_mapUpCorner.X + p.X + 1;
                    int screenPlacementY = m_mapUpCorner.Y + p.Y + 1;
                    if (IsDrawableTile(screenPlacementX, screenPlacementY))
                        screen.SetCharBackground(screenPlacementX, screenPlacementY, TCODColorPresets.DarkRed);
                }
                foreach (ICharacter c in m_monsterFOV.Keys)
                {
                    foreach (Point p in m_monsterFOV[c])
                    {
                        int screenPlacementX = m_mapUpCorner.X + p.X + 1;
                        int screenPlacementY = m_mapUpCorner.Y + p.Y + 1;
                        if (IsDrawableTile(screenPlacementX, screenPlacementY))
                        {
                            Color currentColor = screen.GetCharBackground(screenPlacementX, screenPlacementY);
                            screen.SetCharBackground(screenPlacementX, screenPlacementY, Color.Interpolate(currentColor, GetColorForMonster(c), .6));
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