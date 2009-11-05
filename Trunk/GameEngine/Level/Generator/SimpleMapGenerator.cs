using System;
using System.Collections.Generic;
using libtcodWrapper;

namespace Magecrawl.GameEngine.Level.Generator
{
    internal class SimpleMapGenerator : MapGeneratorBase, IDisposable
    {
        internal SimpleMapGenerator() : base()
        {
        }    

        internal Map GenerateMap()
        {
            Map newMap = new Map(m_random.GetRandomInt(20, 40), m_random.GetRandomInt(20, 40));
            return newMap;
        }
    }
}
