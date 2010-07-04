using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine.Level
{
    internal struct MapTile : IXmlSerializable
    {
        // Be careful here. Any increased size here baloons the map size by a significant size.
        private TerrainType m_type;
        private bool m_visited;
        private byte m_scratch;
        
        // Temporary field used by algorthisms crawling over the map.
        internal byte Scratch 
        {
            get
            {
                return m_scratch;
            }
            set
            {
                m_scratch = value;
            }
        }

        internal MapTile(Interfaces.TerrainType type)
        {
            m_type = type;
            m_visited = false;
            m_scratch = 0;
        }

        internal MapTile(MapTile t)
        {
            m_type = t.m_type;
            m_visited = t.Visited;
            m_scratch = t.Scratch;
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
        
        #region SaveLoad

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