using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public enum TerrainType : byte
    {
        Wall, Floor
    }

    public interface IMap
    {
        int Width
        {
            get;
        }

        int Height
        {
            get;
        }

        IEnumerable<IMapObject> MapObjects
        {
            get;
        }

        IEnumerable<ICharacter> Monsters
        {
            get;
        }

        IEnumerable<Pair<IItem, Point>> Items
        {
            get;
        }

        TerrainType GetTerrainAt(int x, int y);
        TerrainType GetTerrainAt(Point p);
        bool IsPointOnMap(int x, int y);
        bool IsPointOnMap(Point p);
        bool IsVisitedAt(Point p);
    }
}
