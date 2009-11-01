using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Items;

namespace Magecrawl.GameEngine.MapObjects
{
    internal sealed class TreasureChest : OperableMapObject
    {
        private Point m_position;

        public TreasureChest()
            : this(Point.Invalid)
        {
        }

        public TreasureChest(Point position)
        {
            m_position = position;
        }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.TreasureChest;
            }
        }

        public override Point Position
        {
            get
            {
                return m_position;
            }
        }

        public override bool IsSolid
        {
            get 
            {
                return true;
            }
        }

        public override bool CanOperate
        {
            get 
            {
                return true;
            }
        }

        public override void Operate()
        {
            // Remove me
            CoreGameEngine.Instance.Map.RemoveMapItem(this);
            Item newItem = CoreGameEngine.Instance.ItemFactory.CreateItem("Wooden Sword");
            CoreGameEngine.Instance.Map.AddItem(new Pair<Items.Item, Point>(newItem, Position));
        }

        #region SaveLoad

        public override void ReadXml(XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", "TreasureChest");
            m_position.WriteToXml(writer, "Position");
        }

        #endregion
    }
}
