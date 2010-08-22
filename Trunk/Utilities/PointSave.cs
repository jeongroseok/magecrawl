using System.Xml;

namespace Magecrawl.Utilities
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