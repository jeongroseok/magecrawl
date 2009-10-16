using System;
using System.Collections.Generic;
using Utilities;

namespace GameEngine
{
    public sealed class Player : Interfaces.IPlayer
    {
        private Point m_position;

        public Player(int x, int y)
        {
            m_position = new Point(x, y);
        }

        public Point Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }
    }
}
