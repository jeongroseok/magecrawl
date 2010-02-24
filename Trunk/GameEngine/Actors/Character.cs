using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Affects;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal class Character : ICharacter, IXmlSerializable
    {
        public Point Position { get; internal set; }

        public int CT { get; internal set; }

        public int CurrentHP { get; internal set; }

        public int MaxHP { get; internal set; }

        public string Name { get; internal set; }

        public int Vision { get; internal set; }

        public double CTIncreaseModifier { get; set; }

        public double CTCostModifierToMove { get; internal set; }

        public double CTCostModifierToAct { get; internal set; }

        public bool IsAlive
        {
            get
            {
                return CurrentHP > 0;
            }
        }

        private WeaponBase m_equipedWeapon;
        public IWeapon CurrentWeapon
        {
            get
            {
                if (m_equipedWeapon == null)
                    return new MeleeWeapon(this);
                return m_equipedWeapon;
            }
        }

        private WeaponBase m_secondaryWeapon;
        public IWeapon SecondaryWeapon
        {
            get
            {
                if (m_secondaryWeapon == null)
                    return new MeleeWeapon(this);
                return m_secondaryWeapon;
            }
        }

        private int m_uniqueID;
        public int UniqueID
        {
            get
            {
                return m_uniqueID;
            }
        }

        private static int s_idCounter = 0;
        
        protected List<AffectBase> m_affects;

        internal Character() : this(String.Empty, Point.Invalid, 0, 0, 0, 0, 0, 0)
        {
        }

        internal Character(string name, Point p, int maxHP, int visionRange) : this(name, p, maxHP, maxHP, visionRange, 1.0, 1.0, 1.0)
        {
        }

        internal Character(string name, Point p, int hp, int maxHP, int visionRange, double ctIncreaseModifer, double ctMoveCost, double ctActCost)
        {
            Position = p;
            CT = 0;
            CurrentHP = hp;
            MaxHP = maxHP;
            Vision = visionRange;
            Name = name;
            m_equipedWeapon = null;
            m_secondaryWeapon = null;

            CTIncreaseModifier = ctIncreaseModifer;
            CTCostModifierToMove = ctMoveCost;
            CTCostModifierToAct = ctActCost;

            m_affects = new List<AffectBase>();
            
            m_uniqueID = s_idCounter;
            s_idCounter++;
        }

        virtual internal IItem Equip(IItem item)
        {
            if (item is IWeapon)
                return EquipWeapon((IWeapon)item);

            throw new System.InvalidOperationException("Don't know how to equip - " + item.GetType());
        }

        virtual internal IItem Unequip(IItem item)
        {
            if (item is IWeapon)
                return UnequipWeapon();
            throw new System.InvalidOperationException("Don't know how to unequip - " + item.GetType());
        }

        private IWeapon EquipWeapon(IWeapon weapon)
        {
            if (weapon == null)
                throw new System.ArgumentException("EquipWeapon - Null weapon");
            WeaponBase currentWeapon = weapon as WeaponBase;

            IWeapon oldWeapon = UnequipWeapon();

            currentWeapon.Owner = this;
            m_equipedWeapon = currentWeapon;
            if (m_equipedWeapon.IsRanged)
                m_equipedWeapon.IsLoaded = true;

            return oldWeapon;
        }

        private IWeapon UnequipWeapon()
        {
            if (m_equipedWeapon == null)
                return null;
            WeaponBase oldWeapon = m_equipedWeapon as WeaponBase;
            oldWeapon.Owner = null;
            m_equipedWeapon = null;
            return oldWeapon;
        }

        internal IWeapon EquipSecondaryWeapon(IWeapon weapon)
        {
            if (weapon == null)
                throw new System.ArgumentException("EquipSecondaryWeapon - Null weapon");
            WeaponBase currentWeapon = weapon as WeaponBase;

            IWeapon oldWeapon = UnequipSecondaryWeapon();

            currentWeapon.Owner = this;
            m_secondaryWeapon = currentWeapon;
            if (m_secondaryWeapon.IsRanged)
                m_secondaryWeapon.IsLoaded = true;

            return oldWeapon;
        }

        public IWeapon UnequipSecondaryWeapon()
        {
            if (m_secondaryWeapon == null)
                return null;
            WeaponBase oldWeapon = m_secondaryWeapon as WeaponBase;
            oldWeapon.Owner = null;
            m_secondaryWeapon = null;
            return oldWeapon;
        }

        internal virtual double CTCostModifierToAttack
        {
            get
            {
                return CurrentWeapon.CTCostToAttack;
            }
        }

        public virtual double Evade
        {
            get
            {
                return CombatDefenseCalculator.CalculateEvade(this);
            }
        }

        public virtual double Defense
        {
            get
            {
                return CombatDefenseCalculator.CalculateDefense(this);
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
        public virtual DiceRoll MeleeDamage
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public virtual double MeleeSpeed
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

        public IEnumerable<AffectBase> Affects
        {
            get
            {
                return m_affects;
            }
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

            m_equipedWeapon = ReadWeaponFromSave(reader);
            m_secondaryWeapon = ReadWeaponFromSave(reader);

            CTIncreaseModifier = reader.ReadElementContentAsDouble();
            CTCostModifierToMove = reader.ReadElementContentAsDouble();
            CTCostModifierToAct = reader.ReadElementContentAsDouble();
       
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

            WriteWeaponToSave(writer, m_equipedWeapon);
            WriteWeaponToSave(writer, m_secondaryWeapon);

            writer.WriteElementString("CTIncraseModifier", CTIncreaseModifier.ToString());
            writer.WriteElementString("CTCostModifierToMove", CTCostModifierToMove.ToString());
            writer.WriteElementString("CTCostModifierToAct", CTCostModifierToAct.ToString());

            ListSerialization.WriteListToXML(writer, m_affects.ToList(), "Affect");

            m_affects.ForEach(a => a.Apply(this));
        }

        private WeaponBase ReadWeaponFromSave(XmlReader reader)
        {
            return (WeaponBase)Item.ReadXmlEntireNode(reader, this);
        }

        private void WriteWeaponToSave(XmlWriter writer, WeaponBase weapon)
        {
            Item.WriteXmlEntireNode(weapon, "EquipedWeapon", writer);
        }

        #endregion
    }
}