using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using libtcod;

namespace Magecrawl.Utilities
{
    public class Preferences
    {
        private Dictionary<string, object> m_preferences;
        public static readonly Preferences Instance = new Preferences();

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

        // Should things like FPS meter, opening story, and stuff be shown
        public bool DebuggingMode
        {
            get
            {
                return (bool)m_preferences["DebuggingMode"];
            }
        }

        // For all properties not exposed by "nice" helper functions
        public object this[string s]
        {
            get
            {
                return m_preferences[s];
            }
        }

        private void SetupDefaultSettings()
        {
#if DEBUG
            m_preferences["DebuggingMode"] = true;
#else
            m_preferences["DebuggingMode"] = false;
#endif
            if (PlatformFinder.IsRunningOnMac())
                m_preferences["UseAltInsteadOfCtrlForRunning"] = true;
            else
                m_preferences["UseAltInsteadOfCtrlForRunning"] = false;
            m_preferences["DisableAutoTargetting"] = false;
            m_preferences["BumpToOpenDoors"] = false;
            m_preferences["BumpToAttack"] = false;
            m_preferences["Keymapping"] = "Arrows";
            m_preferences["CustomKeymappingFilename"] = "";
            m_preferences["ShowAttackRolls"] = false;
            m_preferences["DebugRangedAttack"] = false;
            m_preferences["UseSavegameCompression"] = true;
            m_preferences["PermaDeath"] = true;
            m_preferences["Fullscreen"] = false;
            m_preferences["SinglePressOperate"] = false;
            m_preferences["FloorColorVisible"] = new TCODColor(42, 42, 42);
            m_preferences["FloorColorNotVisible"] = new TCODColor(15, 15, 15);
            m_preferences["WallColorVisible"] = new TCODColor(83, 41, 0);
            m_preferences["WallColorNotVisible"] = new TCODColor(40, 20, 0);
        }

        private void LoadSettings()
        {
            XMLResourceReaderBase.ParseFileNotInResourcesDir("Preferences.xml", ReadFileCallback);
        }

        private void ReadFileCallback(XmlReader reader, object data)
        {
            // We didn't find our preference file...
            if (reader.LocalName != "Preferences")
                throw new System.IO.FileNotFoundException();
            
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
                    case "SinglePressOperate":
                    case "Fullscreen":
                    case "PermaDeath":
                    case "UseSavegameCompression":
                    case "ShowAttackRolls":
                    case "BumpToAttack":
                    case "BumpToOpenDoors":
                    case "DisableAutoTargetting":
                    case "UseAltInsteadOfCtrlForRunning":
                        ReadBooleanData(reader, reader.LocalName);
                        break;
                    case "FloorColorVisible":
                    case "FloorColorNotVisible":
                    case "WallColorVisible":
                    case "WallColorNotVisible":
                        ReadColorData(reader, reader.LocalName);
                        break;
                    case "Keymapping":
                    case "CustomKeymappingFilename":
                        ReadStringData(reader, reader.LocalName);
                        break;
                }
            }
        }

        private void ReadStringData(XmlReader reader, string preferenceName)
        {
            reader.Read();
            m_preferences[preferenceName] = reader.Value;
        }

        private void ReadColorData(XmlReader reader, string preferenceName)
        {
            reader.Read();
            string[] colorParts = reader.Value.Split(',');
            m_preferences[preferenceName] = new TCODColor(Byte.Parse(colorParts[0]), Byte.Parse(colorParts[1]), Byte.Parse(colorParts[2]));
        }

        private void ReadBooleanData(XmlReader reader, string preferenceName)
        {
            reader.Read();
            m_preferences[preferenceName] = Boolean.Parse(reader.Value);
        }
    }
}
