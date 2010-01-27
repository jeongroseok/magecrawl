using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;

namespace Magecrawl.GameEngine.Armor
{
    internal abstract class ArmorBase : ItemWithOwner, IArmor
    {
        ArmorWeight m_weight;
        internal ArmorBase(string name, ArmorWeight weight, string itemDescription, string flavorText)
            : base(null, name, itemDescription, flavorText)
        {
            m_weight = weight;
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        public ArmorWeight Weight
        {
            get
            {
                return m_weight;
            }
        }
    }
}
