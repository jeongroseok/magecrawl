using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace Magecrawl.GameEngine.SaveLoad
{
    internal static class SaveLoadHelpers
    {
        internal static void SaveGameXML(string fileName, object root)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Create, isf))
                {
                    using (TextWriter sw = new StreamWriter(isfs))
                    {
                        XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
                        s.Serialize(sw, root);
                    }
                }
            }
        }

        internal static void SaveGameCompressed(string fileName, object root)
        {
            // Needs better implementation for silverlight
            SaveGameXML(fileName, root);
        }

        internal static void LoadGameXML(string fileName)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Open, isf))
                {
                    XmlSerializer s = new XmlSerializer(typeof(SaveLoadCore));
                    s.Deserialize(isfs);
                }
            }
        }

        internal static void LoadGameCompressed(string fileName)
        {
            // Needs better implemntation for silverlight
            LoadGameXML(fileName);
        }
    }
}