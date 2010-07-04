using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal sealed class MapObjectFactory
    {
        internal MapObject CreateMapObject(string name)
        {
            return CreateMapObject(name, Point.Invalid);
        }

        internal MapObject CreateMapObject(string name, Point position)
        {
            Type objectType = TypeLocator.GetTypeToMake(typeof(MapObjectFactory), "Magecrawl.GameEngine.MapObjects", name);
            if (objectType == null)
                throw new System.InvalidOperationException("Create Map Object: Don't know how to create type of : " + name);

            return (MapObject)Activator.CreateInstance(objectType, position);
        }
    }
}
