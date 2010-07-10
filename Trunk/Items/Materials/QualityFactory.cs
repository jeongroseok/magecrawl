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
    internal class QualityFactory
    {
        private List<Quality> m_qualityMapping;

        private int m_lowQuality;
        private int m_highQuality;

        internal QualityFactory()
        {
            LoadMappings();
            m_lowQuality = int.MaxValue;
            m_highQuality = int.MinValue;
            foreach (Quality q in m_qualityMapping)
            {
                m_lowQuality = Math.Min(m_lowQuality, q.LevelAdjustment);
                m_highQuality = Math.Max(m_highQuality, q.LevelAdjustment);
            }
        }

        internal Quality GetQuality(string name)
        {
            return m_qualityMapping.Where(q => q.Name == name).FirstOrDefault();
        }

        internal Quality GetRandomQuality()
        {
            int levelAdjustment = DiscreteRandomNumberOnNormalCurve.GetNumber(.5, 2, m_lowQuality, m_highQuality);

            return m_qualityMapping.Where(q => q.LevelAdjustment == levelAdjustment).FirstOrDefault();
        }

        internal Quality GetQualityInRange(int low, int high)
        {
            int coersedLow = Math.Min(Math.Max(m_lowQuality, low), m_highQuality);
            int coersedHigh = Math.Max(Math.Min(m_highQuality, high), m_lowQuality);
            int levelAdjustment = DiscreteRandomNumberOnNormalCurve.GetNumber(.5, 2, coersedLow, coersedHigh);

            return m_qualityMapping.Where(q => q.LevelAdjustment == levelAdjustment).FirstOrDefault();
        }

        internal Quality GetQualityNoHigherThan(int high)
        {
            int coersedHigh = Math.Max(Math.Min(m_highQuality, high), m_lowQuality);
            int levelAdjustment = DiscreteRandomNumberOnNormalCurve.GetNumber(.5, 2, m_lowQuality, coersedHigh);

            return m_qualityMapping.Where(q => q.LevelAdjustment == levelAdjustment).FirstOrDefault();
        }

        private void LoadMappings()
        {
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_qualityMapping = new List<Quality>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "Quality.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // Items element
            if (reader.LocalName != "Qualities")
            {
                throw new System.InvalidOperationException("Bad quality file");
            }

            bool inAttributes = false;
            Quality currentQuality = null;
            string attributeName = null;
            string attributeValue = null;
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Qualities")
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
                            currentQuality.Attributes.Add(attributeName, attributeValue);
                        attributeName = null;
                        attributeValue = null;
                    }
                }
                else if (reader.LocalName == "Quality")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        int levelAdjustment = Int32.Parse(reader.GetAttribute("LevelAdjustment"));
                        currentQuality = new Quality(reader.GetAttribute("Name"), reader.GetAttribute("Description"), levelAdjustment);
                        m_qualityMapping.Add(currentQuality);
                    }
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
