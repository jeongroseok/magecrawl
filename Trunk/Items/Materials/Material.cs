using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Items.Materials
{
    internal class Material
    {        
        internal Material(string name, int level)
        {
            MaterialName = name;
            Level = level;
            FullItemNamed = new Dictionary<string, string>();
            Descriptions = new Dictionary<string, string>();
            Attributes = new Dictionary<string, Dictionary<string, string>>();
            MaterialAttributes = new Dictionary<string, string>();
        }

        public string MaterialName { get; private set; }

        public int Level { get; private set; }

        // Description of item when made of material - e.g. "Carved from a simple hard wood."
        public Dictionary<string, string> Descriptions { get; private set; }
        
        // Name of item when made of material - e.g. "Bronze Sword"
        public Dictionary<string, string> FullItemNamed { get; private set; }

        // Attributes of item when type of material - e.g. Attributes["Sword"]["Foo"]
        public Dictionary<string, Dictionary<string, string>> Attributes;

        // Attributes of base material - e.g. MaterialAttributes["DamageBonus"]
        public Dictionary<string, string> MaterialAttributes;

        public override string ToString()
        {
            return "Material - " + MaterialName;
        }
    }
}
