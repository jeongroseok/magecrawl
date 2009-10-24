using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Magecrawl.Utilities
{
    public struct DiceRoll
    {
        private static Random random = new Random();

        public short Rolls;
        public short DiceFaces;
        public short ToAdd;
        public short Multiplier;

        public DiceRoll(short rolls, short diceFaces)
        {
            Rolls = rolls;
            DiceFaces = diceFaces;
            ToAdd = 0;
            Multiplier = 1;
        }

        public DiceRoll(short rolls, short diceFaces, short toAdd)
        {
            Rolls = rolls;
            DiceFaces = diceFaces;
            ToAdd = toAdd;
            Multiplier = 1;
        }

        public DiceRoll(short rolls, short diceFaces, short toAdd, short multiplier)
        {
            Rolls = rolls;
            DiceFaces = diceFaces;
            ToAdd = toAdd;
            Multiplier = multiplier;
        }

        public short Roll()
        {
            short total = 0;
            for (short i = 0; i < Rolls; i++)
            {
                total += (short)(random.Next(DiceFaces) + 1);
            }
            return (short)(Multiplier * (total + ToAdd));
        }

        public int RollMaxDamage()
        {
            if (Multiplier != 0)
                return (int)Math.Round((double)(Multiplier * (DiceFaces * Rolls)) + ToAdd);
            else
                return (DiceFaces * Rolls) + ToAdd;
        }

        public string PrintableString
        {
            get
            {
                bool hasMult = Multiplier != 0;
                bool hasConstant = ToAdd != 0;

                string multiplierFrontString = hasMult ? Multiplier.ToString() + "*" : String.Empty;
                string frontParen = hasMult || hasConstant ? "(" : String.Empty;
                string endParen = hasMult || hasConstant ? ")" : String.Empty;
                string constantSign = ToAdd >= 0 ? "+" : "-";
                string constantEnd = hasConstant ? constantSign + ToAdd.ToString() : String.Empty;

                return string.Format("{0}{1}{2}d{3}{4}{5}", multiplierFrontString, frontParen, Rolls.ToString(), DiceFaces.ToString(), endParen, constantEnd);
            }
        }

        #region SaveLoad

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            reader.ReadStartElement();
            Rolls = (short)reader.ReadContentAsInt();
            reader.ReadEndElement();

            reader.ReadStartElement();
            DiceFaces = (short)reader.ReadContentAsInt();
            reader.ReadEndElement();

            reader.ReadStartElement();
            Multiplier = (short)reader.ReadContentAsFloat();
            reader.ReadEndElement();

            reader.ReadStartElement();
            ToAdd = (short)reader.ReadContentAsInt();
            reader.ReadEndElement();

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("DamageRoll");
            writer.WriteElementString("Rolls", Rolls.ToString());
            writer.WriteElementString("NumberFaces", DiceFaces.ToString());
            writer.WriteElementString("Multiplier", Multiplier.ToString());
            writer.WriteElementString("AddSub", ToAdd.ToString());
            writer.WriteEndElement();
        }

        #endregion
    }
}
