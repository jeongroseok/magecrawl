using System.Xml;
using libtcod;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Interfaces;
using Magecrawl.Items;
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

        public override string Name
        {
            get
            {
                return "TreasureChest";
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
            internal set
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
            CoreGameEngine.Instance.Map.RemoveMapItem(this);
            CoreGameEngine.Instance.SendTextOutput(string.Format("{0} opens a Treasure Chest", actor.Name));

            TreasureGenerator.GenerateTreasureChestTreasure(actor, Position);
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
