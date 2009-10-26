using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine
{
    internal sealed class MapTile : IMapTile, IXmlSerializable
    {
        private TerrainType m_type;
        private bool m_visited;

        internal MapTile()
        {
            m_type = TerrainType.Wall;
            m_visited = false;
        }

        internal MapTile(Interfaces.TerrainType type)
        {
            m_type = type;
            m_visited = false;
        }

        public TerrainType Terrain
        {
            get 
            {
                return m_type;
            }
            internal set
            {
                m_type = value;
            }
        }

        public bool Visited
        {
            get
            {
                return m_visited;
            }
            internal set
            {
                m_visited = value;
            }
        }
        
        #region

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            m_type = (TerrainType)Enum.Parse(typeof(TerrainType), reader.ReadElementContentAsString());
            m_visited = bool.Parse(reader.ReadElementContentAsString());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", m_type.ToString());
            writer.WriteElementString("Visited", m_visited.ToString());
        }
        #endregion
    }
}