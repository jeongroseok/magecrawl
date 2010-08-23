using System;
using System.Collections.Generic;
using System.Text;

namespace Magecrawl.Maps.Generator.Stitch
{
    internal sealed class StitchtogeatherMapGraphGenerator
    {
        private StitchRatio m_stitchRatio;

        internal StitchtogeatherMapGraphGenerator()
        {
            m_stitchRatio = new StitchRatio(75);
        }

        internal MapNode GenerateMapGraph()
        {
            MapNode graphHead = new MapNode(MapNodeType.Entrance);

            Queue<MapNode> nodesToHandle = new Queue<MapNode>();
            nodesToHandle.Enqueue(graphHead);

            while (nodesToHandle.Count > 0)
            {
                MapNode currentNode = nodesToHandle.Dequeue();

                for (int i = 0; i < NumberOfNeighborsToGenerate(currentNode.Type); ++i)
                {
                    MapNodeType nodeTypeToGenerate = m_stitchRatio.GetNewNode(currentNode.Type);
                    AddNeighborsToNode(currentNode, nodeTypeToGenerate, nodesToHandle);
                }
            }

            ClearMapNodeScratch(graphHead);
            StripEmptyHallWays(graphHead);

            return graphHead;
        }

        private int NumberOfNeighborsToGenerate(MapNodeType current)
        {
            int neighbors = MapChunk.NumberOfNeighbors(current);
            switch (current)
            {
                case MapNodeType.None:
                case MapNodeType.Entrance:
                    return neighbors;
                case MapNodeType.Hall:
                case MapNodeType.MainRoom:
                case MapNodeType.SideRoom:
                case MapNodeType.TreasureRoom:
                    return neighbors - 1;
                case MapNodeType.NoneGivenYet:
                default:
                    throw new ArgumentException("NumberOfNeighborsToGenerate - No valid number of neighbors");
            }
        }

        internal void PrintMapGraph(MapNode current, int indent)
        {
            if (current.Scratch == 3)
                return;

            current.Scratch = 3;

            StringBuilder tabString = new StringBuilder();
            tabString.Append('\t', indent);
            System.Console.Out.WriteLine(tabString + current.Type.ToString() + " " + current.UniqueID.ToString());

            foreach (MapNode n in current.Neighbors)
                PrintMapGraph(n, indent + 1);
        }

        private List<MapNode> GenerateMapNodeList(MapNode headNode)
        {
            List<MapNode> nodeList = new List<MapNode>();
            GenerateMapNodeListCore(headNode, nodeList);
            return nodeList;
        }

        private void GenerateMapNodeListCore(MapNode currentNode, List<MapNode> nodeList)
        {
            if (nodeList.Contains(currentNode))
                return;
            nodeList.Add(currentNode);
            foreach (MapNode n in currentNode.Neighbors)
                GenerateMapNodeListCore(n, nodeList);
        }

        public void ClearMapNodeScratch(MapNode graphHead)
        {
            foreach (MapNode n in GenerateMapNodeList(graphHead))
                n.Scratch = 0;
        }

        private void StripEmptyHallWays(MapNode graphHead)
        {
            bool anyRemoved;
            do
            {
                anyRemoved = false;

                List<MapNode> nodeList = GenerateMapNodeList(graphHead);
                foreach (MapNode n in nodeList)
                {
                    if (n.Type == MapNodeType.Hall)
                    {
                        MapNode neightborOne = n.Neighbors[0];
                        MapNode neightborTwo = n.Neighbors[1];
                        if (neightborOne.Type == MapNodeType.None)
                        {
                            neightborTwo.RemoveNeighbor(n);
                            neightborTwo.AddNeighbor(new MapNode(MapNodeType.None));
                            anyRemoved = true;
                        }
                        else if (neightborTwo.Type == MapNodeType.None)
                        {
                            neightborOne.RemoveNeighbor(n);
                            neightborOne.AddNeighbor(new MapNode(MapNodeType.None));
                            anyRemoved = true;
                        }
                    }
                }
            }
            while (anyRemoved);
        }

        private static void AddNeighborsToNode(MapNode parent, MapNodeType type, Queue<MapNode> nodeQueue)
        {
            MapNode neightbor = new MapNode(type);
            parent.AddNeighbor(neightbor);
            neightbor.AddNeighbor(parent);
            nodeQueue.Enqueue(neightbor);
        }
    }
}
