using System;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Armor;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Affects
{
    internal class EarthenArmor : AffectBase
    {
        private ChestArmor m_previousArmor;

        public EarthenArmor() : base(0)
        {
        }

        public EarthenArmor(int strength)
            : base(new DiceRoll(strength, 8, 8, 2).Roll() * CoreTimingEngine.CTNeededForNewTurn)
        {
        }

        public override string Name
        {
            get
            {
                return "Earthen Armor";
            }
        }

        #region SaveLoad

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            m_previousArmor = (ChestArmor)Item.ReadXmlEntireNode(reader, null);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            Item.WriteXmlEntireNode((Item)m_previousArmor, "ChestArmor", writer);            
        }

        #endregion

        public override void Apply(Character appliedTo)
        {
            Player player = appliedTo as Player;
            if (player == null)
                throw new NotImplementedException("Can't apply earth armor to non-players until they have armor");
            ChestArmor itemPreviousEquiped = (ChestArmor)appliedTo.Equip(CoreGameEngine.Instance.ItemFactory.CreateItem("Earthen Armor"));
            ((ArmorBase)player.ChestArmor).CanNotUnequip = true;

            // On load, we saved read the previous item, but the character himself doesn't know yet what he was wearing, so don't overwrite it.
            // See Character/Player ReadXML ordering.
            if (!(m_previousArmor != null && itemPreviousEquiped == null))
                m_previousArmor = itemPreviousEquiped;
        }

        public override void Remove(Character removedFrom)
        {
            Player player = removedFrom as Player;
            if (player == null)
                throw new NotImplementedException("Can't apply earth armor to non-players until they have armor");
            IItem shouldBeEarthenArmor = player.Unequip(player.ChestArmor);
            if (m_previousArmor != null)
                player.Equip(m_previousArmor);
            if (shouldBeEarthenArmor.DisplayName != "Earthen Armor")
                throw new InvalidOperationException("Earthen Armor got removed before Remove()?");
        }
    }
}
