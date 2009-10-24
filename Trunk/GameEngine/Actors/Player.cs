using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class Player : Character, Interfaces.IPlayer, IXmlSerializable
    {
        public Player() : base()
        {
        }

        public Player(int x, int y) : base(x, y, 10, 10, 4, "Donblas")
        {
        }

        public IWeapon CurrentWeapon
        {
            get
            {
                //return new GameEngine.Weapons.MeleeWeapon();
                return new GameEngine.Weapons.SimpleBow();
            }
        }

        #region SaveLoad

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            base.ReadXml(reader);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Player");
            base.WriteXml(writer);
            writer.WriteEndElement();
        }

        #endregion
    }
}
