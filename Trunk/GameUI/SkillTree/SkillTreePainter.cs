using System.IO;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameUI.SkillTree
{
    internal class SkillTreePainter : MapPainterBase
    {
        internal bool Enabled { get; set; }

        private int m_width;
        private int m_height;
        private char[,] m_array;
        private TCODConsole m_offscreenConsole;

        private const int UpperLeft = 5;
        private const int ScreenWidth = 70;
        private const int ScreenHeight = 50;
        private static Point ScreenCenter = new Point(((ScreenWidth - 1) / 2), ((ScreenHeight - 1) / 2));

        internal SkillTreePainter()
        {
            Enabled = false;
            CursorPosition = new Point(0,0);
            string fileName = Path.Combine(Path.Combine(AssemblyDirectory.CurrentAssemblyDirectory, "Resources"), "SkillTree.dat");
            using(StreamReader s = new StreamReader(fileName))
            {
                string [] sizeLine = s.ReadLine().Split(' ');
                m_width = int.Parse(sizeLine[0]);
                m_height = int.Parse(sizeLine[1]);
                int initialX = int.Parse(sizeLine[2]);
                int initialY = int.Parse(sizeLine[3]);
                CursorPosition = new Point(initialX, initialY);
                m_array = new char[m_width, m_height];
                m_offscreenConsole = new TCODConsole(m_width,  m_height);
                for(int j = 0 ; j < m_height ; ++j)
                {
                    string line = s.ReadLine();
                    for(int i = 0 ; i < m_width ; ++i)
                    {
                        m_array[i, j] = line[i];
                    }
                }
            }
        }
        
        public override void DrawNewFrame (TCODConsole screen)
        {
            if (Enabled)
            {
                DrawOffSceenConsole();
                int lowX = CursorPosition.X - (ScreenWidth / 2);
                int lowY = CursorPosition.Y - (ScreenHeight / 2);
                screen.printFrame(UpperLeft, UpperLeft, ScreenWidth, ScreenHeight, true, TCODBackgroundFlag.Set, "Skill Tree");
                TCODConsole.blit(m_offscreenConsole, lowX, lowY, ScreenWidth - 2, ScreenHeight - 2, screen, UpperLeft + 1, UpperLeft + 1);
                screen.setCharBackground(ScreenCenter.X + UpperLeft + 1, ScreenCenter.Y + UpperLeft + 1, TCODColor.darkGrey);

                screen.print(50, 50, m_cursorPosition.ToString());
            }
        }

        private void DrawOffSceenConsole()
        {
            for (int i = 0 ; i < m_width ; ++i)
            {
                for (int j = 0 ; j < m_height ; ++j)
                {
                    m_offscreenConsole.putChar(i, j, m_array[i,j], TCODBackgroundFlag.None);
                }
            }
        }

        public override void UpdateFromNewData (IGameEngine engine, Point mapUpCorner, Point centerPosition)
        {
        }

        private Point m_cursorPosition;
        internal Point CursorPosition
        {
            get
            {
                return m_cursorPosition;
            }
            set
            {
                m_cursorPosition = value;
            }
        }
    }
}
