using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class Map : Interfaces.IMap, IXmlSerializable
    {
        private int m_width;
        private int m_height;
        private MapTile[,] m_map;
        private List<IMapObject> m_mapObjects;

        internal Map()
        {
            m_width = -1;
            m_height = -1;
        }

        internal Map(int width, int height)
        {
            m_width = width;
            m_height = height;
            m_map = new MapTile[width, height];
            m_mapObjects = new List<IMapObject>();

            CreateDemoMap(width, height);
        }

        public int Width
        {
            get
            {
                return m_width;
            }
        }

        public int Height
        {
            get
            {
                return m_height;
            }
        }

        public IEnumerable<IMapObject> MapObjects
        {
            get 
            {
                return m_mapObjects;
            }
        }

        public IMapTile this[int width, int height]
        {
            get 
            {
                return m_map[width, height];
            }
        }

        private void CreateDemoMap(int width, int height)
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    TerrainType type = TerrainType.Floor;
                    if (i == 0 || j == 0 || i == (m_width - 1) || j == (m_height - 1))
                        type = TerrainType.Wall;

                    m_map[i, j] = new MapTile(type);
                }
            }

            m_map[30, 1].Terrain = TerrainType.Wall;
            m_mapObjects.Add(new MapDoor(new Point(30, 2)));
            m_map[30, 3].Terrain = TerrainType.Wall;
        }

        #region SaveLoad
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            m_width = reader.ReadElementContentAsInt();
            m_height = reader.ReadElementContentAsInt();
            m_map = new MapTile[m_width, m_height];

            for (int i = 0; i < m_width; ++i)
            {
                for (int j = 0; j < m_height; ++j)
                {
                    reader.ReadStartElement();
                    char c = reader.ReadContentAsString()[0];
                    reader.ReadEndElement();
                    m_map[i, j] = new MapTile();
                    m_map[i, j].CovertFromChar(c);
                }
            }

            // Read Map Features
            m_mapObjects = new List<IMapObject>();

            ReadListFromXMLCore readDel = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                MapObject newObj = MapObject.CreateMapObjectFromTypeString(typeString);
                newObj.ReadXml(reader);
                m_mapObjects.Add(newObj);
            });
            ReadListFromXML(reader, readDel);

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Map");
            writer.WriteElementString("Width", m_width.ToString());
            writer.WriteElementString("Height", m_height.ToString());

            for (int i = 0; i < m_width; ++i)
            {
                for (int j = 0; j < m_height; ++j)
                {
                    writer.WriteElementString("Elt", m_map[i, j].ConvertToChar().ToString());
                }
            }

            WriteListToXML(writer, m_mapObjects.ConvertAll<MapObject>(new Converter<IMapObject, MapObject>(delegate(IMapObject m) { return m as MapObject; })), "MapObjects");

            writer.WriteEndElement();
        }

        public static void WriteListToXML<T>(XmlWriter writer, List<T> list, string listName) where T : IXmlSerializable
        {
            writer.WriteStartElement(listName + "List");
            writer.WriteElementString("Count", list.Count.ToString());
            for (int i = 0; i < list.Count; ++i)
            {
                IXmlSerializable current = list[i];
                writer.WriteStartElement(string.Format(listName + "{0}", i));
                current.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public delegate void ReadListFromXMLCore(XmlReader reader);

        public static void ReadListFromXML(XmlReader reader, ReadListFromXMLCore del)
        {
            reader.ReadStartElement();

            int mapFeatureListLength = reader.ReadElementContentAsInt();

            for (int i = 0; i < mapFeatureListLength; ++i)
            {
                reader.ReadStartElement();

                del(reader);

                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        #endregion
    }
}
