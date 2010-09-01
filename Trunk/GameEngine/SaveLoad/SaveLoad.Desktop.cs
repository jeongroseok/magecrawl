using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace Magecrawl.GameEngine.SaveLoad
{
    internal static class SaveLoadHelpers
    {
        internal static void SaveGameXML(string fileName, object root)
        {
            using (TextWriter w = new StreamWriter(fileName))
            {
                XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
                s.Serialize(w, root);
            }
        }

        internal static void SaveGameCompressed(string fileName, object root)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (TextWriter memoryWriter = new StreamWriter(memoryStream))
                {
                    XmlSerializer xmlSerial = new XmlSerializer(typeof(SaveLoadCore));
                    xmlSerial.Serialize(memoryWriter, root);
                    memoryWriter.Close();

                    FileStream outputFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                    GZipStream compressedStream = new GZipStream(outputFile, CompressionMode.Compress, true);

                    byte[] byteArray = memoryStream.ToArray();

                    compressedStream.Write(byteArray, 0, byteArray.Length);

                    compressedStream.Close();
                    outputFile.Close();
                }
            }
        }

        internal static void LoadGameXML(string fileName)
        {
            XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
            TextReader r = new StreamReader(fileName);
            s.Deserialize(r);
            r.Close();
        }

        internal static void LoadGameCompressed(string fileName)
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