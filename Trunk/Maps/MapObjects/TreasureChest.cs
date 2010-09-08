using System.Xml;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;
using Magecrawl.EngineInterfaces;

namespace Magecrawl.Maps.MapObjects
{
    public sealed class TreasureChest : OperableMapObject
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

        public override string Name
        {
            get
            {
                return "Treasure Chest";
            }
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
            set
            {
                m_position = value;
            }
        }

        public override bool IsSolid
        {
            get 
            {
                return true;
            }
        }

        public override bool IsTransarent
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

        public override void Operate(ICharacter actor)
        {
            // Remove me first
            CoreGameEngineInstance.Instance.Map.RemoveMapItem(this);
            CoreGameEngineInstance.Instance.SendTextOutput(string.Format("{0} opens a Treasure Chest", actor.Name));

            CoreGameEngineInstance.Instance.TreasureGenernator.GenerateTreasureChestTreasure(actor, Position);
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
