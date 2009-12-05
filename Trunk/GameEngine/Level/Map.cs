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

namespace Magecrawl.GameEngine.Level
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
            m_width = width;
            m_height = height;
            m_map = new MapTile[m_width, m_height];
            for (int i = 0; i < m_width; ++i)
            {
                for (int j = 0; j < m_height; ++j)
                {
                    m_map[i, j] = new MapTile();
                }
            }
        }

        internal void ClearMap()
        {
            for (int i = 0; i < m_width; ++i)
            {
                for (int j = 0; j < m_height; ++j)
                {
                    m_map[i, j].Terrain = TerrainType.Wall;
                }
            }
            m_mapObjects.Clear();
            m_monsterList.Clear();
            m_items.Clear();
        }

        // Doesn't implement all of ICloneable, just copies mapTiles
        internal void CopyMap(Map sourceMap)
        {
            if(m_width != sourceMap.m_width || m_height != sourceMap.m_height)
                throw new InvalidOperationException("CopyMap of different size");

            for (int i = 0; i < m_width; ++i)
            {
                for (int j = 0; j < m_height; ++j)
                {
                    m_map[i,j].Terrain = sourceMap.m_map[i, j].Terrain;
                }
            }
        }

        // This assumes that all creatures/tiles/items are in that range and that's all that's in map
        internal void TrimToSubset(Point upperLeft, Point lowerRight)
        {
            m_width = lowerRight.X - upperLeft.X + 1;
            m_height = lowerRight.Y - upperLeft.Y + 1;

            int tempI = 0;
            for (int i = upperLeft.X; i <= lowerRight.X; ++i)
            {
                int tempJ = 0;
                for (int j = upperLeft.Y; j <= lowerRight.Y; ++j)
                {
                    m_map[tempI, tempJ].Terrain = m_map[i, j].Terrain;
                    tempJ++;
                }
                tempI++;
            }

            m_mapObjects.ForEach(o => o.Position -= upperLeft);

            List<Pair<Item, Point>> tempItemList = new List<Pair<Item, Point>>();
            foreach (var item in m_items)
            {
                tempItemList.Add(new Pair<Item, Point>(item.First, item.Second - upperLeft));
            }
            m_items = tempItemList;
            
            m_monsterList.ForEach(m => m.Position -= upperLeft);
        }

        internal void AddMonster(Monster m)
        {
            m_monsterList.Add(m);
        }

        internal bool KillMonster(Monster m)
        {
            return m_monsterList.Remove(m);
        }

        internal void AddMapItem(MapObject item)
        {
            m_mapObjects.Add(item);
        }

        internal bool RemoveMapItem(MapObject item)
        {
            return m_mapObjects.Remove(item);
        }

        internal void AddItem(Pair<Item, Point> item)
        {
            m_items.Add(item);
        }

        internal bool RemoveItem(Pair<Item, Point> item)
        {
            return m_items.Remove(item);
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

        public IMapTile this[Point p]
        {
            get
            {
                return m_map[p.X, p.Y];
            }
        }

        // We can't overload this[], and sometimes we need to set internal attributes :(
        public MapTile GetInternalTile(int width, int height)
        {
            return m_map[width, height];
        }

        public MapTile GetInternalTile(Point p)
        {
            return m_map[p.X, p.Y];
        }

        // This is a debugging tool. It prints out a map to Console Out. Usefor for visualizing a map.
        internal void PrintMapToStdOut()
        {
            for (int j = 0; j < Height; ++j)
            {
                for (int i = 0; i < Width; ++i)
                    System.Console.Out.Write(m_map[i, j].Terrain == TerrainType.Wall ? '#' : '.');
                System.Console.Out.WriteLine();
            }
            System.Console.Out.WriteLine();
        }

        // This is a debugging tool. It prints out a map to Console Out with its scratch values
        internal void PrintScratchMapToStdOut()
        {
            for (int j = 0; j < Height; ++j)
            {
                for (int i = 0; i < Width; ++i)
                    System.Console.Out.Write(ConvertScratchToDebugSymbol(m_map[i, j].Scratch));
                System.Console.Out.WriteLine();
            }
            System.Console.Out.WriteLine();
        }

        private string ConvertScratchToDebugSymbol(int scratch)
        {
            if (scratch == -1)
                return '*'.ToString();
            else if (scratch < 9)
                return scratch.ToString();
            else
                return ((char)((int)'a' + scratch - 9)).ToString();
        }

        public bool IsPointOnMap(Point p)
        {
            return (p.X >= 0) && (p.Y >= 0) && (p.X < Width) && (p.Y < Height);
        }

        public Point CoercePointOntoMap(Point p)
        {
            if (p.X < 0)
                p.X = 0;
            if (p.Y < 0)
                p.Y = 0;
            if (p.X >= Width)
                p.X = Width - 1;
            if (p.Y >= Height)
                p.Y = Height - 1;
            return p;
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
