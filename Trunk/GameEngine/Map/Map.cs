using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.MapObjects;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    internal sealed class Map : Interfaces.IMap, IXmlSerializable
    {
        private int m_width;
        private int m_height;
        private MapTile[,] m_map;
        private List<MapObject> m_mapObjects;
        private List<Monster> m_monsterList;
        private List<Pair<Item, Point>> m_items;

        internal Map()
        {
            m_width = -1;
            m_height = -1;
        }

        internal Map(int width, int height)
        {
            m_mapObjects = new List<MapObject>();
            m_monsterList = new List<Monster>();
            m_items = new List<Pair<Item, Point>>();

            CreateDemoMap();
        }

        internal bool KillMonster(Monster m)
        {
            return m_monsterList.Remove(m);
        }

        internal bool RemoveItem(Pair<Item, Point> item)
        {
            return m_items.Remove(item);
        }

        internal bool RemoveMapItem(MapObject item)
        {
            return m_mapObjects.Remove(item);
        }

        internal void AddItem(Pair<Item, Point> item)
        {
            m_items.Add(item);
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

        public IList<IMapObject> MapObjects
        {
            get 
            {
                return m_mapObjects.OfType<IMapObject>().ToList();
            }
        }

        public IList<ICharacter> Monsters
        {
            get 
            {
                return m_monsterList.OfType<ICharacter>().ToList();
            }
        }

        public IList<Pair<IItem, Point>> Items
        {
            get 
            {
                return m_items.ConvertAll<Pair<IItem, Point>>(new Converter<Pair<Item, Point>, Pair<IItem, Point>>(delegate(Pair<Item, Point> i) { return new Pair<IItem, Point>(i.First, i.Second); }));
            }
        }

        internal IList<Pair<Item, Point>> InternalItems
        {
            get
            {
                return m_items;
            }
        }

        public IMapTile this[int width, int height]
        {
            get 
            {
                return m_map[width, height];
            }
        }

        // We can't overload this[], and sometimes we need to set internal attributes :(
        public MapTile GetInternalTile(int width, int height)
        {
            return m_map[width, height];
        }

        private void CreateDemoMap()
        {
            using (StreamReader reader = File.OpenText("map.txt"))
            {
                Random random = new Random();
                string sizeLine = reader.ReadLine();
                string[] sizes = sizeLine.Split(' ');
                m_width = Int32.Parse(sizes[0]);
                m_height = Int32.Parse(sizes[1]);
                m_map = new MapTile[m_width, m_height];

                for (int j = 0; j < m_height; ++j)
                {
                    string tileLine = reader.ReadLine();
                    for (int i = 0; i < m_width; ++i)
                    {
                        m_map[i, j] = new MapTile();
                        if (tileLine[i] != '#')
                            m_map[i, j].Terrain = TerrainType.Floor;
                        switch (tileLine[i])
                        {
                            case ':':
                                m_mapObjects.Add(new MapDoor(new Point(i, j)));
                                break;
                            case 'M':
                                m_monsterList.Add(new Monster(i, j));
                                break;
                            case '!':
                                m_items.Add(new Pair<Item, Point>(CoreGameEngine.Instance.ItemFactory.CreateItem("Wooden Sword"), new Point(i, j)));
                                break;
                            case '+':
                                m_mapObjects.Add(new TreasureChest(new Point(i, j)));
                                break;
                        }
                    }
                }
            }
        }

        internal bool IsPointOnMap(Point p)
        {
            return (p.X >= 0) && (p.Y >= 0) && (p.X < Width) && (p.Y < Height);
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
                    m_map[i, j] = new MapTile();
                    m_map[i, j].ReadXml(reader);
                }
            }

            // Read Map Features
            m_mapObjects = new List<MapObject>();

            ReadListFromXMLCore readDelegate = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                MapObject newObj = CoreGameEngine.Instance.MapObjectFactory.CreateMapObject(typeString);
                newObj.ReadXml(reader);
                m_mapObjects.Add(newObj);
            });
            ListSerialization.ReadListFromXML(reader, readDelegate);

            // Read Monsters
            m_monsterList = new List<Monster>();

            readDelegate = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                Monster newObj = CoreGameEngine.Instance.MonsterFactory.CreateMonster(typeString);
                newObj.ReadXml(reader);
                m_monsterList.Add(newObj);
            });
            ListSerialization.ReadListFromXML(reader, readDelegate);

            // Read Items
            m_items = new List<Pair<Item, Point>>();

            readDelegate = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                Item newItem = CoreGameEngine.Instance.ItemFactory.CreateItem(typeString);
                Point position = new Point();
                position = position.ReadXml(reader);
                newItem.ReadXml(reader);
                m_items.Add(new Pair<Item, Point>(newItem, position));
            });
            ListSerialization.ReadListFromXML(reader, readDelegate);

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
                    m_map[i, j].WriteXml(writer);
                }
            }

            ListSerialization.WriteListToXML(writer, m_mapObjects, "MapObjects");

            ListSerialization.WriteListToXML(writer, m_monsterList, "Monsters");

            ListSerialization.WriteListToXML(writer, m_items, "Items");

            writer.WriteEndElement();
        }

        #endregion
    }
}
