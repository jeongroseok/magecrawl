using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

#if SILVERLIGHT
using System;
using System.Windows;
using System.Windows.Resources;
#endif

namespace Magecrawl.Utilities
{
    public delegate void ReadFileSpecifics(XmlReader reader, object data);

    public static class XMLResourceReaderBase
    {
        public static void ParseFile(string fileName, ReadFileSpecifics readFileCallback)
        {
            StreamReader reader = GetFileStream(Path.Combine("Resources", fileName));
            ParseFile(reader, readFileCallback, null);
        }

        public static void ParseFileNotInResourcesDir(string fileName, ReadFileSpecifics readFileCallback)
        {
            StreamReader reader = GetFileStream(fileName);
            ParseFile(reader, readFileCallback, null);
        }

        public static StreamReader GetFileStream(string fileName)
        {
#if SILVERLIGHT
            string fixedFilename = fileName.Replace("\\", "/");
            StreamResourceInfo sr = Application.GetResourceStream(new Uri(fixedFilename, UriKind.Relative));
            return new StreamReader(sr.Stream);
#else
            return new StreamReader(fileName);
#endif
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
