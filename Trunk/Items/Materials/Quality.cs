using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.Items.Materials
{
    internal class Quality
    {
        internal Quality(string name, string description, int level)
        {
            Name = name;
            LevelAdjustment = level;
            Description = description;
            Attributes = new Dictionary<string, string>();
        }

        public string Name { get; private set; }
        public int LevelAdjustment { get; private set; }
        public string Description { get; private set; }
        public Dictionary<string, string> Attributes;

        public override string ToString()
        {
            return "Quality - " + Name;
        }
    }
}
