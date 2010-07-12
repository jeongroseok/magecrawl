using System.Collections.Generic;

namespace Magecrawl.Items.Materials
{
    internal class ConsumableEffect
    {
        internal ConsumableEffect(string effectName, string spellName, int itemLevel, int casterLevel)
        {
            EffectName = effectName;
            SpellName = spellName;
            ItemLevel = itemLevel;
            CasterLevel = casterLevel;            
            Descriptions = new Dictionary<string, string>();
            Types = new Dictionary<string, string>();
            DisplayNames = new Dictionary<string, string>();
        }

        public string EffectName { get; private set; }
        public int ItemLevel { get; private set; }
        public int CasterLevel { get; internal set; }
        public string SpellName { get; private set; }
        public Dictionary<string, string> Types { get; private set; }
        public Dictionary<string, string> DisplayNames { get; private set; }
        public Dictionary<string, string> Descriptions { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", EffectName, SpellName, ItemLevel);
        }
    }
}
