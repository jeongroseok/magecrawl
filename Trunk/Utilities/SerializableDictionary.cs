// Taken from: http://weblogs.asp.net/pwelter34/archive/2006/05/03/444961.aspx

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    public SerializableDictionary() : base()
    {
    }

    public SerializableDictionary(SerializableDictionary <TKey, TValue> x) : base(x)
    {
    }

    #region IXmlSerializable Members

    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(System.Xml.XmlReader reader)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

        reader.ReadStartElement();
        // We read this out to make sure the dictionary has a begin/end tag
        reader.ReadElementContentAsInt();

        while (!(reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.Name == "Dictionary"))
        {
            reader.ReadStartElement("Item");
            reader.ReadStartElement("Key");

            TKey key = (TKey)keySerializer.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("Value");
            TValue value = (TValue)valueSerializer.Deserialize(reader);
            reader.ReadEndElement();

            this.Add(key, value);

            reader.ReadEndElement();
            reader.MoveToContent();
        }

        reader.ReadEndElement();
    }

    public void WriteXml(System.Xml.XmlWriter writer)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

        writer.WriteStartElement("Dictionary");
        // We write this out to make sure the dictionary has a begin/end tag
        writer.WriteElementString("NumberElements", this.Keys.Count.ToString());
        foreach (TKey key in this.Keys)
        {
            writer.WriteStartElement("Item");
            writer.WriteStartElement("Key");

            keySerializer.Serialize(writer, key);
            
            writer.WriteEndElement();

            writer.WriteStartElement("Value");

            TValue value = this[key];

            valueSerializer.Serialize(writer, value);

            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }
    #endregion

}