using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;

namespace Magecrawl.GameEngine.Armor
{
    internal abstract class ArmorBase : ItemWithOwner, IArmor
    {
        internal ArmorBase(string name, string itemDescription, string flavorText) : base(null, name, itemDescription, flavorText)
        {
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
