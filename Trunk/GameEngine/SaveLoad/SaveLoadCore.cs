using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.Maps;
using Magecrawl.Maps.MapObjects;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.SaveLoad
{
    public class SaveLoadCore : IXmlSerializable 
    {
        private const int SaveVersion = 1;

        internal bool SaveGame(string filename)
        {
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                if ((bool)Preferences.Instance["UseSavegameCompression"])
                    SaveLoadHelpers.SaveGameCompressed(filename, this);
                else
                    SaveLoadHelpers.SaveGameXML(filename, this);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previousCulture; 
            }
            
            return true;
        }

        internal bool LoadGame(string filename)
        {
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                if ((bool)Preferences.Instance["UseSavegameCompression"])
                    SaveLoadHelpers.LoadGameCompressed(filename);
                else
                    SaveLoadHelpers.LoadGameXML(filename);

                if ((bool)Preferences.Instance["PermaDeath"])
                    File.Delete(filename);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previousCulture;
            }

            return true;
        }

        #region SaveLoad

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            reader.ReadStartElement();
            string versionString = reader.ReadElementContentAsString();

            if (versionString != SaveVersion.ToString())
                throw new System.InvalidOperationException("Attemping to load bad savefile.");

            CoreGameEngine.Instance.TurnCount = reader.ReadElementContentAsInt();

            int numberOfLevelsToLoad = reader.ReadElementContentAsInt();

            int currentLevel = reader.ReadElementContentAsInt();

            Dictionary<int, Map> loadingDungeon = new Dictionary<int, Map>();
            
            for (int i = 0; i < numberOfLevelsToLoad; i++)
            {
                Map loadMap = new Map();

                reader.ReadStartElement();
                loadMap.ReadXml(reader);
                reader.ReadEndElement();
                loadingDungeon[i] = loadMap;
            }

            StairsMapping.Instance.ReadXml(reader);
            
            Player loadPlayer = new Player();
            loadPlayer.ReadXml(reader);

            CoreGameEngine.Instance.SetWithSaveData(loadPlayer, loadingDungeon, currentLevel);

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("MagecrawlSaveFile");
            writer.WriteElementString("Version", SaveVersion.ToString());

            writer.WriteElementString("TurnCount", CoreGameEngine.Instance.TurnCount.ToString());
            writer.WriteElementString("DungonLength", CoreGameEngine.Instance.NumberOfLevels.ToString());
            writer.WriteElementString("CurrentLevel", CoreGameEngine.Instance.CurrentLevel.ToString());

            for (int i = 0; i < CoreGameEngine.Instance.NumberOfLevels; i++)
            {
                writer.WriteStartElement(string.Format("Map{0}", i));
                CoreGameEngine.Instance.GetSpecificFloor(i).WriteXml(writer);
                writer.WriteEndElement();
            }

            StairsMapping.Instance.WriteXml(writer);

            (CoreGameEngine.Instance.Player as Player).WriteXml(writer);

            writer.WriteEndElement();
        }

        #endregion
    }
}
