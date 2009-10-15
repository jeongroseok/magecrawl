using System;
using System.Collections.Generic;

namespace GameEngine
{
    public sealed class Player : Interfaces.IPlayer
    {
        private Point m_position;

        public Player()
        {
            m_position = new Point(0, 0);
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
