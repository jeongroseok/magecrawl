using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;

namespace Magecrawl.GameUI.Utilities
{
    // Sometimes we use a large number of colors in tight loops. This class caching the instances
    // so we don't recalculate or P/Invoke them again
    public class ColorCache
    {
        public static ColorCache Instance = new ColorCache();

        private Dictionary<string, TCODColor> m_cache;

        private ColorCache()
        {
            m_cache = new Dictionary<string,TCODColor>();
        }

        public TCODColor this[string i]
        {
            get { return m_cache[i]; }
            set { m_cache[i] = value; }
        }
    }
}
