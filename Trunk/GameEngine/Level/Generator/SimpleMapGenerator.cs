using System;
using System.Collections.Generic;
using libtcodWrapper;

namespace Magecrawl.GameEngine.Level.Generator
{
    internal class SimpleMapGenerator : MapGeneratorBase, IDisposable
    {
        private TCODRandom m_random;

        internal SimpleMapGenerator()
        {
            m_random = new TCODRandom();
        }
        
        public void Dispose()
        {
            if (m_random != null)
                m_random.Dispose();
            m_random = null;
        }        

        internal Map GenerateMap()
        {
            Map newMap = new Map(m_random.GetRandomInt(20, 40), m_random.GetRandomInt(20, 40));
            return newMap;
        }
    }
}
