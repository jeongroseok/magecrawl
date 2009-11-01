using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Affects;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class Character : Interfaces.ICharacter, IXmlSerializable
    {
        internal Character() : this(0, 0, 0, 0, 0, 0, 0, String.Empty)
        {
        }

        internal Character(int x, int y, int hp, int maxHP, int visionRange, int magic, int maxMagic, string name)
        {
            m_position = new Point(x, y);
            m_CT = 0;
            m_hp = hp;
            m_maxHP = maxHP;
            m_mp = magic;
            m_maxMP = maxMagic;
            m_visionRange = visionRange;
            m_name = name; 
            m_uniqueID = s_idCounter;
            CTIncreaseModifier = 1.0;
            m_affects = new List<AffectBase>();
            s_idCounter++;
        }

        protected Point m_position;
        protected int m_hp;
        protected int m_maxHP;
        protected int m_mp;
        protected int m_maxMP;
        protected string m_name;
        protected int m_visionRange;
        
        protected int m_uniqueID;
        private static int s_idCounter = 0;

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "CT is an acronym")]
        protected int m_CT;

        protected List<AffectBase> m_affects;

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
            get; set;
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
            protected set
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

        public int CurrentMP
        {
            get
            {
                return m_mp;
            }
            internal set
            {
                m_mp = value;
            }
        }

        public int MaxMP
        {
            get
            {
                return m_maxMP;
            }
            internal set
            {
                m_maxMP = value;
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

        // Returns amount actually healed by
        public int Heal(int toHeal)
        {
            int previousHealth = CurrentHP;
            CurrentHP = Math.Min(CurrentHP + toHeal, MaxHP);
            return CurrentHP - previousHealth;
        }

        // Everyone should override these. 
        // I want character to have a constructor to reduce copying, but there are some things that should be overridded
        public virtual IWeapon CurrentWeapon
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public virtual DiceRoll MeleeDamage
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public virtual void IncreaseCT(int increase)
        {
            CT += increase;
        }

        public virtual void DecreaseCT(int decrease)
        {
            CT -= decrease;

            foreach (AffectBase affect in m_affects)
            {
                affect.DecreaseCT(decrease);
            }
            IEnumerable<AffectBase> toRemove = m_affects.Where(a => a.CTLeft <= 0);
            foreach (AffectBase affect in toRemove)
            {
                affect.Remove(this);
            }
            m_affects.RemoveAll(a => a.CTLeft <= 0);
        }

        public void AddAffect(AffectBase affectToAdd)
        {
            m_affects.Add(affectToAdd);
            affectToAdd.Apply(this);
        }

        #region SaveLoad

        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
            m_hp = reader.ReadElementContentAsInt();
            m_maxHP = reader.ReadElementContentAsInt();
            m_mp = reader.ReadElementContentAsInt();
            m_maxMP = reader.ReadElementContentAsInt();
            m_name = reader.ReadElementContentAsString();
            m_CT = reader.ReadElementContentAsInt();
            m_visionRange = reader.ReadElementContentAsInt();
            m_uniqueID = reader.ReadElementContentAsInt();

            ListSerialization.ReadListFromXML(
                reader, 
                innerReader =>
                {
                    string typeName = reader.ReadElementContentAsString();
                    AffectBase affect = AffectFactory.CreateAffectBaseObject(typeName);
                    affect.ReadXml(reader);
                    AddAffect(affect);
                });
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            foreach (AffectBase affect in m_affects)
            {
                affect.Remove(this);
            }

            Position.WriteToXml(writer, "Position");
            writer.WriteElementString("CurrentHP", m_hp.ToString());
            writer.WriteElementString("MaxHP", m_maxHP.ToString());
            writer.WriteElementString("CurrentMagic", m_mp.ToString());
            writer.WriteElementString("MaxMagic", m_maxMP.ToString());
            writer.WriteElementString("Name", m_name);
            writer.WriteElementString("CT", m_CT.ToString());
            writer.WriteElementString("VisionRange", m_visionRange.ToString());
            writer.WriteElementString("UniqueID", m_uniqueID.ToString());

            ListSerialization.WriteListToXML(writer, m_affects.ToList(), "Affect");
            foreach (AffectBase affect in m_affects)
            {
                affect.Apply(this);
            }
        }

        #endregion
    }
}