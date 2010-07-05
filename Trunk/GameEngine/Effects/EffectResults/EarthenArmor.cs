using System;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Armor;
using Magecrawl.GameEngine.Items;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    internal class EarthenArmor : EffectResult
    {
        private ChestArmor m_previousArmor;

        public EarthenArmor()
        {
        }

        public EarthenArmor(int strength)
        {
        }

        internal override string Name
        {
            get
            {
                return "Earthen Armor";
            }
        }

        internal override bool IsPositiveEffect
        {
            get
            {
                return true;
            }
        }

        internal override int DefaultMPSustainingCost
        {
            get
            {
                return 40;
            }
        }

        internal override int DefaultEffectLength
        {
            get 
            {
                return (new DiceRoll(1, 10, 20)).Roll() * CoreTimingEngine.CTNeededForNewTurn;    //20-30 turns
            }
        }

        internal override bool ProvidesEquipment(IArmor armor)
        {
            return armor.DisplayName == "Earthen Armor";
        }

        internal override void Apply(Character appliedTo)
        {
            Player player = appliedTo as Player;
            if (player == null)
                throw new NotImplementedException("Can't apply earth armor to non-players.");
            ChestArmor itemPreviousEquiped = (ChestArmor)appliedTo.Equip(CoreGameEngine.Instance.ItemFactory.CreateItem("Earthen Armor"));
            ((ArmorBase)player.ChestArmor).Summoned = true;

            // On load, we saved read the previous item, but the character himself doesn't know yet what he was wearing, so don't overwrite it.
            // See Character/Player ReadXML ordering.
            if (!(m_previousArmor != null && itemPreviousEquiped == null))
                m_previousArmor = itemPreviousEquiped;
        }

        internal override void Remove(Character removedFrom)
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

        #region SaveLoad

        internal override void ReadXml(System.Xml.XmlReader reader)
        {
            m_previousArmor = (ChestArmor)Item.ReadXmlEntireNode(reader, null);
        }

        internal override void WriteXml(System.Xml.XmlWriter writer)
        {
            Item.WriteXmlEntireNode((Item)m_previousArmor, "ChestArmor", writer);
        }

        #endregion
    }
}
