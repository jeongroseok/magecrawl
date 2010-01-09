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
        public Point Position { get; internal set; }

        public int CT { get; internal set; }

        public int CurrentHP { get; internal set; }

        public int MaxHP { get; internal set; }

        public string Name { get; internal set; }

        public int Vision { get; internal set; }

        protected int m_uniqueID;
        public int UniqueID
        {
            get
            {
                return m_uniqueID;
            }
        }

        private static int s_idCounter = 0;
        
        protected List<AffectBase> m_affects;

        internal virtual double CTIncreaseModifier { get; set; }

        internal Character() : this(String.Empty, Point.Invalid, 0, 0)
        {
        }

        internal Character(string name, Point p, int maxHP, int visionRange) : this(name, p, maxHP, maxHP, visionRange)
        {
        }

        internal Character(string name, Point p, int hp, int maxHP, int visionRange)
        {
            Position = p;
            CT = 0;
            CurrentHP = hp;
            MaxHP = maxHP;
            Vision = visionRange;
            Name = name; 
            CTIncreaseModifier = 1.0;
            m_affects = new List<AffectBase>();
            
            m_uniqueID = s_idCounter;
            s_idCounter++;
        }
        
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

        internal virtual double CTCostModifierToAttack
        {
            get
            {
                return CurrentWeapon.CTCostToAttack;
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

            m_affects.ForEach(a => a.DecreaseCT(decrease));
            
            foreach (AffectBase affect in m_affects.Where(a => a.CTLeft <= 0))
                affect.Remove(this);

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
            Position = Position.ReadXml(reader);
            CurrentHP = reader.ReadElementContentAsInt();
            MaxHP = reader.ReadElementContentAsInt();
            Name = reader.ReadElementContentAsString();
            CT = reader.ReadElementContentAsInt();
            Vision = reader.ReadElementContentAsInt();
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
            m_affects.ForEach(a => a.Remove(this));

            Position.WriteToXml(writer, "Position");
            writer.WriteElementString("CurrentHP", CurrentHP.ToString());
            writer.WriteElementString("MaxHP", MaxHP.ToString());
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("CT", CT.ToString());
            writer.WriteElementString("VisionRange", Vision.ToString());
            writer.WriteElementString("UniqueID", m_uniqueID.ToString());

            ListSerialization.WriteListToXML(writer, m_affects.ToList(), "Affect");

            m_affects.ForEach(a => a.Apply(this));
        }

        #endregion
    }
}