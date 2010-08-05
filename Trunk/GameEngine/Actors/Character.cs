using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Effects;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Interfaces;
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
 
            CTIncreaseModifier = ctIncreaseModifer;
            CTCostModifierToMove = ctMoveCost;
            CTCostModifierToAct = ctActCost;

            m_effects = new List<StatusEffect>();
            
            m_uniqueID = s_idCounter;
            s_idCounter++;
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
        public abstract void DamageStamina(int dmg);

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
            List<ShortTermEffect> shortTermEffects = m_effects.OfType<ShortTermEffect>().ToList();

            shortTermEffects.ForEach(e => e.DecreaseCT(CT + decrease, CT));
            
            foreach (StatusEffect effect in shortTermEffects.Where(e => e.CTLeft <= 0))
                RemoveEffect(effect);

            // Remove any long term effects that have been "dismissed"
            List<LongTermEffect> longTermEffectsToRemove = m_effects.OfType<LongTermEffect>().Where(a => a.Dismissed).ToList();
            foreach (LongTermEffect effect in longTermEffectsToRemove)
                RemoveEffect(effect);
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

        internal virtual int GetTotalAttributeValue(string attribute)
        {
            return 0;
        }

        internal virtual bool HasAttribute(string attribute)
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
            Vision = reader.ReadElementContentAsInt();
            m_uniqueID = reader.ReadElementContentAsInt();

            CTIncreaseModifier = reader.ReadElementContentAsDouble();
            CTCostModifierToMove = reader.ReadElementContentAsDouble();
            CTCostModifierToAct = reader.ReadElementContentAsDouble();
       
            ListSerialization.ReadListFromXML(
                reader, 
                innerReader =>
                {
                    string typeName = reader.ReadElementContentAsString();
                    bool longTerm = Boolean.Parse(reader.ReadElementContentAsString());
                    StatusEffect effect = EffectFactory.CreateEffectBaseObject(typeName, longTerm);
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
            writer.WriteElementString("VisionRange", Vision.ToString());
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