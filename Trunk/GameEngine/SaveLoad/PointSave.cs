using System;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.SaveLoad
{
    public static class PointExtensions
    {
        public static Point WriteToXml(this Point p, XmlWriter w, string xmlTag)
        {
            w.WriteElementString(xmlTag, p.X.ToString() + "," + p.Y.ToString());
            return p;
        }

        public static Point ReadXml(this Point p, XmlReader r)
        {
            string positionString = r.ReadElementContentAsString();
            p.X = int.Parse(positionString.Split(',')[0]);
            p.Y = int.Parse(positionString.Split(',')[1]);
            return p;
        }
    }
}