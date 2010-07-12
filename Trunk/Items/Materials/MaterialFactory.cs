using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Magecrawl.Utilities;

namespace Magecrawl.Items.Materials
{
    internal class MaterialFactory
    {
        private Dictionary<string, Material> m_materialMapping;
        private Dictionary<string, List<Material>> m_validMaterialsForType;

        internal MaterialFactory()
        {
            LoadMappings();
        }

        internal Material GetMaterial(string type, string materialName)
        {
            Material material = m_materialMapping[materialName];
            if (!material.FullItemNamed.ContainsKey(type))
                return null;
            return material;
        }

        internal Material GetRandomMaterial(string type)
        {
            if(!m_validMaterialsForType.ContainsKey(type))
                return null;

            return m_validMaterialsForType[type].Randomize()[0];
        }

        internal Material GetMaterialInLevelRange(string type, int low, int high)
        {
            if (!m_validMaterialsForType.ContainsKey(type))
                return null;

            List<Material> validMaterials = m_validMaterialsForType[type].Where(m => m.Level >= low && m.Level <= high).ToList();
            if (validMaterials.Count == 0)
                return null;
            return validMaterials.Randomize()[0];
        }

        private void LoadMappings()
        {
            m_materialMapping = new Dictionary<string, Material>();
            m_validMaterialsForType = new Dictionary<string, List<Material>>();
            XMLResourceReaderBase.ParseFile("Materials.xml", ReadFileCallback);
        }

        void ReadFileCallback(XmlReader reader, object data)
        {
            if (reader.LocalName != "Materials")
                throw new System.InvalidOperationException("Bad material file");

            bool inAttributes = false;
            Material currentMaterial = null;
            string attributeName = null;
            string attributeValue = null;
            string lastItemType = null;
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Materials")
                    break;

                if (inAttributes)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        attributeName = reader.LocalName;
                    }
                    else if (reader.NodeType == XmlNodeType.Text)
                    {
                        attributeValue = reader.Value;
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (attributeName != null)
                            currentMaterial.Attributes[lastItemType].Add(attributeName, attributeValue);
                        attributeName = null;
                        attributeValue = null;
                    }
                }
                else if (reader.LocalName == "Material")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        int level = Int32.Parse(reader.GetAttribute("Level"));
                        string name = reader.GetAttribute("Name");
                        currentMaterial = new Material(name, level);

                        string damageBonus = reader.GetAttribute("DamageBonus");
                        if (damageBonus != null)
                            currentMaterial.MaterialAttributes["DamageBonus"] = damageBonus;

                        string extraCTCost = reader.GetAttribute("ExtraCTCost");
                        if (extraCTCost != null)
                            currentMaterial.MaterialAttributes["ExtraCTCost"] = extraCTCost;

                        string staminaBonus = reader.GetAttribute("StaminaBonus");
                        if (staminaBonus != null)
                            currentMaterial.MaterialAttributes["StaminaBonus"] = staminaBonus;

                        m_materialMapping.Add(currentMaterial.MaterialName, currentMaterial);
                    }
                }
                else if (reader.LocalName == "ValidItem" && reader.NodeType == XmlNodeType.Element)
                {
                    string type = reader.GetAttribute("Type");
                    lastItemType = type;

                    string description = reader.GetAttribute("Description");
                    string fullItemName = reader.GetAttribute("FullItemName");

                    currentMaterial.Descriptions.Add(type, description);
                    currentMaterial.FullItemNamed.Add(type, fullItemName);

                    currentMaterial.Attributes[type] = new Dictionary<string, string>();

                    if (m_validMaterialsForType.ContainsKey(type))
                        m_validMaterialsForType[type].Add(currentMaterial);
                    else
                        m_validMaterialsForType[type] = new List<Material>() { currentMaterial };
                }
                if (reader.LocalName == "Attributes")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        inAttributes = true;
                    else if (reader.NodeType == XmlNodeType.EndElement)
                        inAttributes = false;
                }
            }
        }
    }
}
