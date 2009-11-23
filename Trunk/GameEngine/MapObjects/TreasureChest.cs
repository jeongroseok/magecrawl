using System.Xml;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

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
            // Remove me first
            CoreGameEngine.Instance.Map.RemoveMapItem(this);

            // Now drop a random item
            // This should be level dependent
            Item newItem = CoreGameEngine.Instance.ItemFactory.CreateRandomItem();
            CoreGameEngine.Instance.Map.AddItem(new Pair<Items.Item, Point>(newItem, Position));
        }

        #region SaveLoad

        public override void ReadXml(XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Type", "Treasure Chest");
            m_position.WriteToXml(writer, "Position");
        }

        #endregion
    }
}
