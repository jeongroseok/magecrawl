using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class Character : Interfaces.ICharacter, IXmlSerializable
    {
        internal Character()
        {
            m_position = new Point(-1, -1);
            m_CT = 0;
            m_hp = 0;
            m_maxHP = 0;
            m_visionRange = 0;
            m_name = String.Empty;
            m_uniqueID = s_idCounter;
            s_idCounter++;
        }

        internal Character(int x, int y, int hp, int maxHP, int visionRange, string name)
        {
            m_position = new Point(x, y);
            m_CT = 0;
            m_hp = hp;
            m_maxHP = maxHP;
            m_visionRange = visionRange;
            m_name = name; 
            m_uniqueID = s_idCounter;
            s_idCounter++;
        }

        protected Point m_position;
        protected int m_hp;
        protected int m_maxHP;
        protected string m_name;
        protected int m_visionRange;
        
        protected int m_uniqueID;
        private static int s_idCounter = 0;

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "CT is an acronym")]
        protected int m_CT;

        internal virtual double CTCostModifierToMove
        {
            get
            {
                return 1.0;
            }
        }

        internal virtual double CTCostModifierToAct
        {
            get
            {
                return 1.0;
            }
        }

        internal virtual double CTIncreaseModifier
        {
            get
            {
                return 1.0;
            }
        }

        public Point Position
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

        public int CT
        {
            get
            {
                return m_CT;
            }
            internal set
            {
                m_CT = value;
            }
        }

        public int CurrentHP
        {
            get
            {
                return m_hp;
            }
            internal set
            {
                m_hp = value;
            }
        }

        public int MaxHP
        {
            get
            {
                return m_maxHP;
            }
            internal set
            {
                m_maxHP = value;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            internal set
            {
                m_name = value;
            }
        }

        public int Vision
        {
            get
            {
                return m_visionRange;
            }
            internal set
            {
                m_visionRange = value;
            }
        }

        public int UniqueID
        {
            get
            {
                return m_uniqueID;
            }
        }

        public void Heal(int toHeal)
        {
            CurrentHP = Math.Min(CurrentHP + toHeal, MaxHP);
        }

        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
            m_hp = reader.ReadElementContentAsInt();
            m_maxHP = reader.ReadElementContentAsInt();
            m_name = reader.ReadElementContentAsString();
            m_CT = reader.ReadElementContentAsInt();
            m_visionRange = reader.ReadElementContentAsInt();
            m_uniqueID = reader.ReadElementContentAsInt();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            Position.WriteToXml(writer, "Position");
            writer.WriteElementString("CurrentHP", m_hp.ToString());
            writer.WriteElementString("MaxHP", m_maxHP.ToString());
            writer.WriteElementString("Name", m_name);
            writer.WriteElementString("CT", m_CT.ToString());
            writer.WriteElementString("VisionRange", m_visionRange.ToString());
            writer.WriteElementString("UniqueID", m_uniqueID.ToString());
        }
    }
}