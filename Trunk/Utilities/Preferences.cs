using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Magecrawl.Utilities
{
    public class Preferences
    {
        private Dictionary<string, object> m_preferences;
        private static Preferences m_instance = new Preferences();

        internal Preferences()
        {
            m_preferences = new Dictionary<string, object>();
            
            // First load default settings for everything
            SetupDefaultSettings();

            try
            {
                // Try to load the settings from a file
                LoadSettings();
            }
            catch (Exception)
            {
                // And ignore if the preference files doesn't exist
            }
        }

        public static Preferences Instance
        {
            get
            {
                return m_instance;
            }
        }

        // Should things like FPS meter, opening story, and stuff be shown
        public bool DebuggingMode
        {
            get
            {
                return (bool)m_preferences["DebuggingMode"];
            }
        }

        public bool SinglePressOperate
        {
            get
            {
                return (bool)m_preferences["SinglePressOperate"];
            }
        }

        private void SetupDefaultSettings()
        {
#if DEBUG
            m_preferences["DebuggingMode"] = true;
#else
            m_preferences["DebuggingMode"] = false;
#endif
            m_preferences["SinglePressOperate"] = false;
        }

        private void LoadSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader("Preferences.xml"), settings);
            reader.Read();  // XML declaration
            reader.Read();  // KeyMappings element
            if (reader.LocalName != "Preferences")
            {
                // We didn't find our preference file...
                throw new System.IO.FileNotFoundException();
            }
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    // If we're at an end element, if its the preference section, break, else skip it
                    if (reader.LocalName == "Preferences")
                        break;
                    else
                        continue;
                }

                switch (reader.LocalName)
                {
                    case "DebuggingMode":
                        ReadBooleanData(reader, "DebuggingMode");
                        break;
                    case "SinglePressOperate":
                        ReadBooleanData(reader, "SinglePressOperate");
                        break;
                }
            }
            reader.Close();
        }

        private void ReadBooleanData(XmlReader reader, string preferenceName)
        {
            reader.Read();
            m_preferences[preferenceName] = Boolean.Parse(reader.Value);
        }

    }
}
