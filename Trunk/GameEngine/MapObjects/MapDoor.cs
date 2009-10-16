using System;
using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal sealed class MapDoor : OperableMapObject
    {
        private Point m_position;
        private bool m_opened;

        public MapDoor(Point position)
            : this(position, false)
        {
        }

        public MapDoor(Point position, bool isOpen)
        {
            m_position = position;
            m_opened = isOpen;
        }

        public override MapObjectType Type
        {
            get
            {
                if (m_opened)
                    return MapObjectType.OpenDoor;
                else
                    return MapObjectType.ClosedDoor;
            }
        }

        public override Point Position
        {
            get
            {
                return m_position;
            }
        }

        public override bool IsSolid
        {
            get 
            {
                return !m_opened;
            }
        }

        public override void Operate()
        {
            m_opened = !m_opened;
        }
    }
}
