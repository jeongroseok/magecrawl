using System.Xml;
using libtcod;
using Magecrawl.GameEngine.Actors;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.MapObjects
{
    internal sealed class TreasureChest : OperableMapObject
    {
        private static TCODRandom m_random = new TCODRandom();
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
            
            // Now drop a random item
            // This should be level dependent
            for (int i = 0; i < m_random.getInt(1, 3); ++i)
            {
                Item newItem = CoreGameEngine.Instance.ItemFactory.CreateRandomItem(CoreGameEngine.Instance.CurrentLevel);
                CoreGameEngine.Instance.SendTextOutput(string.Format("{0} finds at {1}", actor.Name, newItem.DisplayName));
                if (actor is Player)
                {
                    ((Player)actor).TakeItem(newItem);
                }
                else
                {
                    CoreGameEngine.Instance.Map.AddItem(new Pair<Items.Item, Point>(newItem, Position));
#if DEBUG
                    throw new System.NotImplementedException("Non players opening chests?");
#endif
                }
            }
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
