using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Magecrawl.Utilities;

namespace Magecrawl.Items.Materials
{
    internal class ComsumableEffectFactory
    {
        private Dictionary<string, ConsumableEffect> m_effectMapping;
        private Dictionary<string, List<ConsumableEffect>> m_validEffectsForType;

        internal ComsumableEffectFactory()
        {
            LoadMappings();
        }

        internal ConsumableEffect GetEffect(string type, string effectName)
        {
            ConsumableEffect effect = m_effectMapping[effectName];
            if (!effect.Types.ContainsKey(type))
                return null;
            return effect;
        }

        internal ConsumableEffect GetRandomEffect(string type)
        {
            if (!m_validEffectsForType.ContainsKey(type))
                return null;

            return m_validEffectsForType[type].Randomize()[0];
        }

        internal ConsumableEffect GetEffectInLevelRange(string type, int low, int high)
        {
            if (!m_validEffectsForType.ContainsKey(type))
                return null;

            List<ConsumableEffect> validEffects = m_validEffectsForType[type].Where(m => m.ItemLevel >= low && m.ItemLevel <= high).ToList();
            if (validEffects.Count == 0)
                return null;
            return validEffects.Randomize()[0];
        }

        private void LoadMappings()
        {
            m_effectMapping = new Dictionary<string, ConsumableEffect>();
            m_validEffectsForType = new Dictionary<string, List<ConsumableEffect>>();    
            XMLResourceReaderBase.ParseFile("Consumables.xml", ReadFileCallback);
        }

        private void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Consumables")
                throw new System.InvalidOperationException("Bad consumables file");

            ConsumableEffect currentEffect = null;
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Consumables")
                    break;

                if (reader.LocalName == "ConsumableEffect" && reader.NodeType == XmlNodeType.Element)
                {
                    int itemLevel = int.Parse(reader.GetAttribute("ItemLevel"));
                    int casterLevel = 1;
                    if (reader.GetAttribute("CasterLevel") != null)
                        casterLevel = int.Parse(reader.GetAttribute("CasterLevel"));
                    currentEffect = new ConsumableEffect(reader.GetAttribute("Name"), reader.GetAttribute("SpellName"), itemLevel, casterLevel);

                    m_effectMapping.Add(currentEffect.EffectName, currentEffect);
                }
                else if (reader.LocalName == "ConsumableInstance" && reader.NodeType == XmlNodeType.Element)
                {
                    string type = reader.GetAttribute("Type");

                    string description = reader.GetAttribute("ItemDescription");
                    string itemName = reader.GetAttribute("Name");

                    currentEffect.Descriptions.Add(type, description);
                    currentEffect.DisplayNames.Add(type, itemName);
                    currentEffect.Types.Add(type, type);

                    if (m_validEffectsForType.ContainsKey(type))
                        m_validEffectsForType[type].Add(currentEffect);
                    else
                        m_validEffectsForType[type] = new List<ConsumableEffect>() { currentEffect };
                }
            }
        }
    }
}
