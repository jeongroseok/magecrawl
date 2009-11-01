using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Magecrawl.Utilities
{
    public struct DiceRoll
    {
        private static Random random = new Random();
        public static DiceRoll Invalid = new DiceRoll(-1, -1, -1, -1);

        public short Rolls;
        public short DiceFaces;
        public short ToAdd;
        public short Multiplier;

        public DiceRoll(int rolls, int diceFaces)
            : this((short)rolls, (short)diceFaces, (short)0, (short)1)
        {
        }

        public DiceRoll(int rolls, int diceFaces, int toAdd)
            : this((short)rolls, (short)diceFaces, (short)toAdd, (short)1)
        {
        }

        public DiceRoll(int rolls, int diceFaces, int toAdd, int multiplier)
            : this((short)rolls, (short)diceFaces, (short)toAdd, (short)multiplier)
        {
        }

        public DiceRoll(short rolls, short diceFaces)
            : this(rolls, diceFaces, (short)0, (short)1)
        {
        }

        public DiceRoll(short rolls, short diceFaces, short toAdd)
            : this(rolls, diceFaces, (short)toAdd, (short)1)
        {
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

        public override string ToString()
        {
            bool hasMult = Multiplier != 1;
            bool hasConstant = ToAdd != 0;

            string multiplierFrontString = hasMult ? Multiplier.ToString() + "*" : String.Empty;
            string frontParen = hasMult || hasConstant ? "(" : String.Empty;
            string endParen = hasMult || hasConstant ? ")" : String.Empty;
            string constantSign = ToAdd >= 0 ? "+" : String.Empty;    // Minus will come with number
            string constantEnd = hasConstant ? constantSign + ToAdd.ToString() : String.Empty;

            // 1d1 doesn't look nice
            if (!hasMult && !hasConstant && Rolls == 1 && DiceFaces == 1)
                return "1";

            return string.Format("{0}{1}{2}d{3}{4}{5}", multiplierFrontString, frontParen, Rolls.ToString(), DiceFaces.ToString(), constantEnd, endParen);
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
