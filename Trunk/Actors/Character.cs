using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.EngineInterfaces;
using Magecrawl.Interfaces;
using Magecrawl.StatusEffects;
using Magecrawl.StatusEffects.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Actors
{
    public abstract class Character : ICharacterCore, IXmlSerializable
    {
        public Point Position { get; set; }

        public int CT { get; internal set; }

        public abstract int CurrentHP { get; }

        public abstract int MaxHP { get; }

        public string Name { get; internal set; }

        public abstract int Vision { get; }

        public abstract IWeapon CurrentWeapon { get; }

        public bool IsAlive
        {
            get
            {
                return CurrentHP > 0;
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

        private double m_ctIncreaseModifier;
        public double CTIncreaseModifier 
        {
            get
            {
                double modifier = GetTotalDoubleAttributeValue("CTIncreaseModifierBonus");
                return modifier == 0 ? m_ctIncreaseModifier : m_ctIncreaseModifier * modifier;
            }
        }

        private double m_ctCostModifierToMove;
        public double CTCostModifierToMove 
        {
            get
            {
                double modifier = GetTotalDoubleAttributeValue("CTCostModifierToMoveBonus");
                return modifier == 0 ? m_ctCostModifierToMove : m_ctCostModifierToMove * modifier;
            }
        }

        private double m_ctCostModifierToAct;
        public double CTCostModifierToAct 
        {
            get
            {
                double modifier = GetTotalDoubleAttributeValue("CTCostModifierToActBonus");
                return modifier == 0 ? m_ctCostModifierToAct : m_ctCostModifierToAct * modifier;
            }
        }

        private static int s_idCounter = 0;
        
        protected List<IStatusEffectCore> m_effects;

        public Character() : this("", Point.Invalid, 0, 0, 0)
        {
            m_effects = new List<IStatusEffectCore>();
        }

        public Character(string name, Point p) : this(name, p, 1.0, 1.0, 1.0)
        {
            m_effects = new List<IStatusEffectCore>();
        }

        public Character(string name, Point p, double ctIncreaseModifer, double ctMoveCost, double ctActCost)
        {
            Position = p;
            CT = 0;
            Name = name;
 
            m_ctIncreaseModifier = ctIncreaseModifer;
            m_ctCostModifierToMove = ctMoveCost;
            m_ctCostModifierToAct = ctActCost;

            m_effects = new List<IStatusEffectCore>();
            
            m_uniqueID = s_idCounter;
            s_idCounter++;
        }

        public virtual double CTCostModifierToAttack
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
        public abstract void DamageJustStamina(int dmg);
        public abstract bool IsDead
        {
            get;
        }

        public abstract DiceRoll MeleeDamage { get; }
        public abstract double MeleeCTCost { get; }

        public virtual void IncreaseCT(int increase)
        {
            CT += increase;
        }

        public virtual void DecreaseCT(int decrease)
        {
            CT -= decrease;

            // Remove short term effects if ct <= 0
            List<IShortTermStatusEffect> shortTermEffects = m_effects.OfType<IShortTermStatusEffect>().ToList();

            shortTermEffects.ForEach(e => e.DecreaseCT(CT + decrease, CT));

            foreach (IShortTermStatusEffect effect in shortTermEffects.Where(e => e.CTLeft <= 0))
                RemoveEffect(effect);

            // Remove any long term effects that have been "dismissed"
            List<ILongTermStatusEffect> longTermEffectsToRemove = m_effects.OfType<ILongTermStatusEffect>().Where(a => a.Dismissed).ToList();
            foreach (ILongTermStatusEffect effect in longTermEffectsToRemove)
                RemoveEffect(effect);
        }

        public virtual void AddEffect(IStatusEffectCore effectToAdd)
        {
            m_effects.Add(effectToAdd);
            effectToAdd.Apply(this);
        }

        public virtual void RemoveEffect(IStatusEffectCore effectToRemove)
        {
            m_effects.Remove(effectToRemove);
            effectToRemove.Remove(this);
        }

        public IEnumerable<IStatusEffectCore> Effects
        {
            get
            {
                return m_effects;
            }
        }

        public virtual int GetTotalAttributeValue(string attribute)
        {
            return 0;
        }

        public virtual double GetTotalDoubleAttributeValue(string attribute)
        {
            return 0;
        }

        public virtual bool HasAttribute(string attribute)
        {
            return false;
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
            m_uniqueID = reader.ReadElementContentAsInt();

            m_ctIncreaseModifier = reader.ReadElementContentAsDouble();
            m_ctCostModifierToMove = reader.ReadElementContentAsDouble();
            m_ctCostModifierToAct = reader.ReadElementContentAsDouble();
       
            ListSerialization.ReadListFromXML(
                reader, 
                innerReader =>
                {
                    string typeName = reader.ReadElementContentAsString();
                    bool longTerm = Boolean.Parse(reader.ReadElementContentAsString());
                    IStatusEffectCore effect = EffectFactory.CreateEffectBaseObject(typeName, longTerm);
                    effect.ReadXml(reader);
                    AddEffect(effect);
                });
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            m_effects.ForEach(a => a.Remove(this));

            Position.WriteToXml(writer, "Position");
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("CT", CT.ToString());
            writer.WriteElementString("UniqueID", m_uniqueID.ToString());

            writer.WriteElementString("CTIncraseModifier", CTIncreaseModifier.ToString());
            writer.WriteElementString("CTCostModifierToMove", CTCostModifierToMove.ToString());
            writer.WriteElementString("CTCostModifierToAct", CTCostModifierToAct.ToString());

            ListSerialization.WriteListToXML(writer, m_effects.ToList(), "Effect");

            m_effects.ForEach(a => a.Apply(this));
        }

        #endregion
    }
}