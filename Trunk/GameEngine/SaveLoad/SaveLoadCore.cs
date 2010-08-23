using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
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
                    SaveGameCompressed(filename);
                else
                    SaveGameXML(filename);
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
                    LoadGameCompressed(filename);
                else
                    LoadGameXML(filename);

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

        private void SaveGameXML(string fileName)
        {
            XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
            TextWriter w = new StreamWriter(fileName);
            s.Serialize(w, this);
            w.Close();
        }

        private void SaveGameCompressed(string fileName)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                TextWriter memoryWriter = new StreamWriter(memoryStream);
                XmlSerializer xmlSerial = new XmlSerializer(typeof(SaveLoadCore));
                xmlSerial.Serialize(memoryWriter, this);
                memoryWriter.Close();

                FileStream outputFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                GZipStream compressedStream = new GZipStream(outputFile, CompressionMode.Compress, true);

                byte[] byteArray = memoryStream.ToArray();

                compressedStream.Write(byteArray, 0, byteArray.Length);

                compressedStream.Close();
                outputFile.Close();
                memoryWriter.Close();
            }
        }

        private void LoadGameXML(string fileName)
        {
            XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
            TextReader r = new StreamReader(fileName);
            s.Deserialize(r);
            r.Close();
        }

        private void LoadGameCompressed(string fileName)
        {
            using (FileStream inputFile = new FileStream(fileName, FileMode.Open))
            {
                using (MemoryStream uncompressedStream = new MemoryStream())
                {
                    using (GZipStream uncompresser = new GZipStream(inputFile, CompressionMode.Decompress))
                    {
                        while (true)
                        {
                            byte[] buffer = new byte[4096];
                            int count = uncompresser.Read(buffer, 0, 4096);
                            if (count > 0)
                                uncompressedStream.Write(buffer, 0, count);
                            else
                                break;
                        }
                    }
                    uncompressedStream.Seek(0, SeekOrigin.Begin);
                    XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
                    TextReader r = new StreamReader(uncompressedStream);
                    s.Deserialize(r);
                    r.Close();
                }
            }
        }
    }
}
