using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Xml;
using System.IO;
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
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_materialMapping = new Dictionary<string, Material>();
            m_validMaterialsForType = new Dictionary<string, List<Material>>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "Materials.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // Items element
            if (reader.LocalName != "Materials")
            {
                throw new System.InvalidOperationException("Bad material file");
            }

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
                        {
                            if (currentMaterial.Attributes.ContainsKey(lastItemType))
                                currentMaterial.Attributes[lastItemType].Add(attributeName, attributeValue);
                            else
                                currentMaterial.Attributes[lastItemType] = new Dictionary<string, string>() { { attributeName, attributeValue } };
                        }
                        attributeName = null;
                        attributeValue = null;
                    }
                }
                else if (reader.LocalName == "Material")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        int level = Int32.Parse(reader.GetAttribute("Level"));
                        currentMaterial = new Material(reader.GetAttribute("Name"), level);
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
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture;
        }
    }
}
