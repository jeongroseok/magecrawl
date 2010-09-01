using System;
using System.Collections.Generic;

namespace Magecrawl.Maps.Generator.Stitch
{
    // Make sure to update MapNode.NodeTypes if you add to this enum
    internal enum MapNodeType
    {
        NoneGivenYet,
        None,
        Entrance,
        Hall,
        MainRoom,
        TreasureRoom,
        SideRoom
    }

    internal class MapNode
    {
        public static List<MapNodeType> NodeTypes
        {
            get
            {
                return new List<MapNodeType>() { MapNodeType.NoneGivenYet, MapNodeType.None, MapNodeType.Entrance, MapNodeType.Hall, 
                    MapNodeType.MainRoom, MapNodeType.TreasureRoom, MapNodeType.SideRoom };
            }
        }

        private static int nextUnqiueID = 1;

        public List<MapNode> Neighbors;
        public MapNodeType Type;
        public bool Generated;
        public int Scratch;
        public int UniqueID;

        internal MapNode()
        {
            Neighbors = new List<MapNode>();
            Type = MapNodeType.NoneGivenYet;
            Generated = false;
            Scratch = 0;

            UniqueID = nextUnqiueID;
            nextUnqiueID++;
        }

        internal MapNode(MapNodeType type)
        {
            Neighbors = new List<MapNode>();
            Type = type;
            Generated = false;
            Scratch = 0;

            UniqueID = nextUnqiueID;
            nextUnqiueID++;
        }

        internal void AddNeighbor(MapNode neighbor)
        {
            Neighbors.Add(neighbor);
        }

        internal void RemoveNeighbor(MapNode neighbor)
        {
            Neighbors.Remove(neighbor);
        }

        public override string ToString()
        {
            return Type.ToString() + " - " + UniqueID.ToString();
        }
    }
}
