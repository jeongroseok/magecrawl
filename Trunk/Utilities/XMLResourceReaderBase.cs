using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace Magecrawl.Utilities
{
    public delegate void ReadFileSpecifics(XmlReader reader, object data);

    public static class XMLResourceReaderBase
    {
        public static void ParseFile(string fileName, ReadFileSpecifics readFileCallback)
        {
            ParseFile(fileName, readFileCallback, null);
        }

        public static void ParseFileNotInResourcesDir(string fileName, ReadFileSpecifics readFileCallback)
        {
            StreamReader stream = new StreamReader(fileName);
            ParseFile(stream, readFileCallback, null);
        }

        public static void ParseFile(string fileName, ReadFileSpecifics readFileCallback, object data)
        {
            StreamReader stream = new StreamReader(Path.Combine("Resources", fileName));
            ParseFile(stream, readFileCallback, data);
        }

        public static void ParseFile(StreamReader stream, ReadFileSpecifics readFileCallback)
        {
            ParseFile(stream, readFileCallback, null);
        }

        public static void ParseFile(StreamReader stream, ReadFileSpecifics readFileCallback, object data)
        {
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(stream, settings);
            reader.Read();  // XML declaration
            reader.Read();  // KeyMappings element

            readFileCallback(reader, data);

            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture; 
        }
    }
}
