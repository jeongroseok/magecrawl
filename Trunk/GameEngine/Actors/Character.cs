using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Effects;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Items;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.GameEngine.Weapons;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal abstract class Character : ICharacter, IXmlSerializable
    {
        public Point Position { get; internal set; }

        public int CT { get; internal set; }

        public abstract int CurrentHP { get; }

        public abstract int MaxHP { get; }

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

        public string DisplayName
        {
            get
            {
                return Name;
            }
        }

        private static int s_idCounter = 0;
        
        protected List<StatusEffect> m_effects;

        internal Character() : this("", Point.Invalid, 0, 0, 0, 0)
        {
            m_effects = new List<StatusEffect>();
        }

        internal Character(string name, Point p, int visionRange) : this(name, p, visionRange, 1.0, 1.0, 1.0)
        {
            m_effects = new List<StatusEffect>();
        }

        internal Character(string name, Point p, int visionRange, double ctIncreaseModifer, double ctMoveCost, double ctActCost)
        {
            Position = p;
            CT = 0;
            Vision = visionRange;
            Name = name;
            m_equipedWeapon = null;
            m_secondaryWeapon = null;

            CTIncreaseModifier = ctIncreaseModifer;
            CTCostModifierToMove = ctMoveCost;
            CTCostModifierToAct = ctActCost;

            m_effects = new List<StatusEffect>();
            
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

        public abstract double Evade { get; }

        // Returns amount actually healed by
        public abstract int Heal(int toHeal, bool magical);

        public abstract void Damage(int dmg);
        public abstract DiceRoll MeleeDamage { get; }
        public abstract double MeleeSpeed { get;}

        public virtual void IncreaseCT(int increase)
        {
            CT += increase;
        }

        public virtual void DecreaseCT(int decrease)
        {
            CT -= decrease;

            // Remove short term effects if ct <= 0
            List<ShortTermEffect> shortTermEffects = m_effects.OfType<ShortTermEffect>().ToList();

            shortTermEffects.ForEach(a => a.DecreaseCT(decrease));
            
            foreach (StatusEffect effect in shortTermEffects.Where(a => a.CTLeft <= 0))
                RemoveEffect(effect);

            // Remove any long term effects that have been "dismissed"
            m_effects.RemoveAll(a => a is LongTermEffect && ((LongTermEffect)a).Dismissed);
        }

        public virtual void AddEffect(StatusEffect effectToAdd)
        {
            m_effects.Add(effectToAdd);
            effectToAdd.Apply(this);
        }

        public virtual void RemoveEffect(StatusEffect effectToRemove)
        {
            m_effects.Remove(effectToRemove);
            effectToRemove.Remove(this);
        }

        public IEnumerable<StatusEffect> Effects
        {
            get
            {
                return m_effects;
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
                    bool longTerm = Boolean.Parse(reader.ReadElementContentAsString());
                    StatusEffect affect = EffectFactory.CreateEffectBaseObject(typeName, longTerm);
                    affect.ReadXml(reader);
                    AddEffect(affect);
                });
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            m_effects.ForEach(a => a.Remove(this));

            Position.WriteToXml(writer, "Position");
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("CT", CT.ToString());
            writer.WriteElementString("VisionRange", Vision.ToString());
            writer.WriteElementString("UniqueID", m_uniqueID.ToString());

            WriteWeaponToSave(writer, m_equipedWeapon);
            WriteWeaponToSave(writer, m_secondaryWeapon);

            writer.WriteElementString("CTIncraseModifier", CTIncreaseModifier.ToString());
            writer.WriteElementString("CTCostModifierToMove", CTCostModifierToMove.ToString());
            writer.WriteElementString("CTCostModifierToAct", CTCostModifierToAct.ToString());

            ListSerialization.WriteListToXML(writer, m_effects.ToList(), "Effect");

            m_effects.ForEach(a => a.Apply(this));
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