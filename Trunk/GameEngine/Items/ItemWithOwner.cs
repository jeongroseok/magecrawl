using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine.Items
{
    internal abstract class ItemWithOwner : Item
    {
        internal ItemWithOwner(ICharacter owner, string name, string itemDescription, string flavorText) : base(name, itemDescription, flavorText)
        {
            Owner = owner;
        }

        internal ICharacter Owner { get; set; }
    }
}
