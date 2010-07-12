using System;
using System.Xml;

namespace Magecrawl.Utilities
{
    public struct DiceRoll
    {
        private static Random random = new Random();
        public static DiceRoll Invalid = new DiceRoll(-1, -1, -1, -1.0);

        public short Rolls;
        public short DiceFaces;
        public double Multiplier;
        public short ToAdd;

        public DiceRoll(int rolls, int diceFaces)
            : this((short)rolls, (short)diceFaces, (short)0, (short)1)
        {
        }

        public DiceRoll(int rolls, int diceFaces, int toAdd)
            : this((short)rolls, (short)diceFaces, (short)toAdd, (short)1)
        {
        }

        public DiceRoll(int rolls, int diceFaces, int toAdd, double multiplier)
            : this((short)rolls, (short)diceFaces, (short)toAdd, multiplier)
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

        public void Add(DiceRoll other)
        {
            if (DiceFaces != other.DiceFaces)
                throw new InvalidOperationException(string.Format("Can't add dice rolls: {0} + {1}", this, other));

            Rolls += other.Rolls;
            ToAdd += other.ToAdd;
            Multiplier += 1 - other.Multiplier;
        }

        public DiceRoll(string s)
        {
            string[] damageParts = s.Split(',');

            Rolls = short.Parse(damageParts[0]);            
            DiceFaces = short.Parse(damageParts[1]);
            
            if (damageParts.Length > 2)
                ToAdd = short.Parse(damageParts[2]);
            else
                ToAdd = 0;

            if (damageParts.Length > 3)
                Multiplier = double.Parse(damageParts[3]);
            else
                Multiplier = 1;
        }

        public DiceRoll(short rolls, short diceFaces, short toAdd, double multiplier)
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
            return (int)Math.Round((double)(Multiplier * (DiceFaces * Rolls)) + ToAdd);
        }

        public override string ToString()
        {
            bool hasMult = Multiplier != 1;
            bool hasConstant = ToAdd != 0;

            string multiplierFrontString = hasMult ? Multiplier.ToString() + "*" : "";
            string frontParen = hasMult || hasConstant ? "(" : "";
            string endParen = hasMult || hasConstant ? ")" : "";
            string constantSign = ToAdd >= 0 ? "+" : "";    // Minus will come with number
            string constantEnd = hasConstant ? constantSign + ToAdd.ToString() : "";

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
            Multiplier = (short)reader.ReadContentAsDouble();
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
