using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.Effects;
using Magecrawl.GameEngine.Magic;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.GameEngine.Skills;
using Magecrawl.Interfaces;
using Magecrawl.Items;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal sealed class Player : Character, IPlayer, IXmlSerializable
    {
        public IArmor ChestArmor { get; internal set; }
        public IArmor Headpiece { get; internal set; }
        public IArmor Gloves { get; internal set; }
        public IArmor Boots { get; internal set; }

        public int SkillPoints { get; internal set; }

        private List<Item> m_itemList;
        private List<Skill> m_skills;

        public int LastTurnSeenAMonster { get; set; }

        public Player() : base()
        {
            m_itemList = null;
            m_skills = null;
            m_currentStamina = 0;
            m_baseMaxStamina = 0;
            m_currentHealth = 0;
            m_baseMaxHealth = 0;
            m_baseCurrentMP = 0;
            m_baseMaxStamina = 0;
            LastTurnSeenAMonster = 0;
            SkillPoints = 0;

            m_itemList = new List<Item>();
            m_skills = new List<Skill>();
        }

        public Player(string name, Point p) : base(name, p, 6)
        {
            m_itemList = new List<Item>();
            m_skills = new List<Skill>();

            m_baseMaxStamina = 0;
            m_currentStamina = m_baseMaxStamina;

            m_baseMaxHealth = 70;
            m_currentHealth = m_baseMaxHealth;

            m_baseMaxMP = 10;
            m_baseCurrentMP = m_baseMaxMP;
            
            LastTurnSeenAMonster = 0; 
        }

        #region HP/MP

        internal void SetHPMPMax()
        {
            m_currentStamina = MaxStamina;
            m_currentHealth = MaxHealth;
            m_baseCurrentMP = MaxMP;
        }
        
        private int m_currentStamina;
        public int CurrentStamina
        {
            get
            {
                return m_currentStamina;
            }
        }
        
        private int m_currentHealth;
        public int CurrentHealth
        {
            get
            {
                return m_currentHealth;
            }
        }

        public override int CurrentHP 
        {
            get
            {
                return CurrentHealth + CurrentStamina;
            }
        }
        
        private int m_baseMaxStamina;
        public int MaxStamina
        {
            get
            {
                int baseMaxStamina = m_baseMaxStamina + CombatDefenseCalculator.CalculateArmorStaminaBonus(this);
                double bonusStamina = 1 + (GetTotalAttributeValue("HPBonus") / 100.0);
                return (int)Math.Round(baseMaxStamina * bonusStamina);
            }
        }

        private int m_baseMaxHealth;
        public int MaxHealth
        {
            get
            {
                return m_baseMaxHealth;
            }
        }

        public override int MaxHP
        {
            get
            {
                return MaxHealth + MaxStamina;
            }
        }

        private int m_baseCurrentMP;
        public int CurrentMP 
        {
            get
            {
                return m_baseCurrentMP;
            }
        }

        // Similar logic is found in Spell::SustainingCost
        public int MaxMP
        {
            get
            {
                return (int)Math.Round(MaxPossibleMP * (1 - ((double)m_effects.OfType<LongTermEffect>().Sum(x => x.MPCost) / 100.0)));
            }
        }

        private int m_baseMaxMP;
        public int MaxPossibleMP
        {
            get
            {
                return m_baseMaxMP + GetTotalAttributeValue("MPBonus");
            }
        }

        private void ResetMaxStaminaIfNowOver()
        {
            if (CurrentStamina > MaxStamina)
                m_currentStamina = MaxStamina;
        }

        private void ResetMaxMPIfNowOver()
        {
            if (CurrentMP > MaxMP)
                m_baseCurrentMP = MaxMP;
        }

        // Returns amount actually healed by
        public override int Heal(int toHeal, bool magical)
        {
            if (toHeal < 0)
                throw new InvalidOperationException("Heal with < 0.");

            int amountOfDamageToHeal = toHeal;
            int amountInTotalHealed = 0;
            if (magical)
            {
                int amountOfHealthMissing = MaxHealth - CurrentHealth;
                if (amountOfHealthMissing > 0)
                {
                    int amountOfHealthToHeal = Math.Min(amountOfDamageToHeal, amountOfHealthMissing);
                    m_currentHealth += amountOfHealthToHeal;
                    amountOfDamageToHeal -= amountOfHealthToHeal;
                    amountInTotalHealed = amountOfHealthToHeal;
                }
            }
            if (amountOfDamageToHeal > 0)
            {
                int amountOfStaminaMissing = MaxStamina - CurrentStamina;
                if (amountOfStaminaMissing > 0)
                {
                    int amountOfStaminaToHeal = Math.Min(amountOfDamageToHeal, amountOfStaminaMissing);
                    m_currentStamina += amountOfStaminaToHeal;
                    amountInTotalHealed += amountOfStaminaToHeal;
                }
            }
            return amountInTotalHealed;
        }

        public override void Damage(int dmg)
        {
            int amountOfDamageLeftToDo = dmg;
            int amountOfDamageToStamina = Math.Min(m_currentStamina, amountOfDamageLeftToDo);
            m_currentStamina -= amountOfDamageToStamina;
            amountOfDamageLeftToDo -= amountOfDamageToStamina;

            if (amountOfDamageLeftToDo > 0)
            {
                int amountOfDamageToHealth = Math.Min(m_currentHealth, amountOfDamageLeftToDo);
                m_currentHealth -= amountOfDamageToHealth;
            }
        }

        public void GainMP(int amount)
        {
            m_baseCurrentMP += amount;
            if (m_baseCurrentMP > MaxMP)
                m_baseCurrentMP = MaxMP;
        }

        public void SpendMP(int amount)
        {
            m_baseCurrentMP -= amount;
        }

        #endregion

        public IEnumerable<ISpell> Spells
        {
            get 
            {
                List<ISpell> returnList = new List<ISpell>();
                foreach (Skill skill in m_skills.Where(s => s.Attributes.ContainsKey("AddSpell")))
                    returnList.Add(SpellFactory.Instance.CreateSpell(skill.Attributes["AddSpell"]));
                return returnList;
            }
        }

        public int SpellStrength(string spellType)
        {
            return 1 + GetTotalAttributeValue(spellType + "Proficiency");
        }

        public bool CouldCastSpell(ISpell spell)
        {
            return (CurrentMP - ((Spell)spell).SustainingCost) >= ((Spell)spell).Cost;
        }

        internal int GetTotalAttributeValue(string attribute)
        {
            int skillBonus = m_skills.Sum(s => s.Attributes.GetNumbericIfAny(attribute));
            int effectBonus = StatusEffects.OfType<StatusEffect>().Where(e => e.ContainsKey(attribute)).Select(e => int.Parse(e.GetAttribute(attribute))).Sum();
            return effectBonus + skillBonus;
        }

        internal bool HasAttribute(string attribute)
        {
            bool existsInSkills = m_skills.Exists(s => s.Attributes.ContainsKey(attribute));
            bool existsInEffects = StatusEffects.OfType<StatusEffect>().Any(e => e.ContainsKey(attribute));
            return existsInSkills || existsInEffects;
        }

        public IEnumerable<IItem> Items
        {
            get
            {
                return m_itemList.ConvertAll<IItem>(i => i).ToList();
            }
        }

        public IEnumerable<IStatusEffect> StatusEffects
        {
            get
            {
                return m_effects.ConvertAll<IStatusEffect>(i => i).ToList();
            }
        }

        public IEnumerable<ISkill> Skills
        {
            get
            {
                return m_skills.ConvertAll<ISkill>(x => x).ToList();
            }
        }

        public override void AddEffect(StatusEffect effectToAdd)
        {
            base.AddEffect(effectToAdd);
            ResetMaxStaminaIfNowOver();
            ResetMaxMPIfNowOver();
        }

        public override void RemoveEffect(StatusEffect effectToRemove)
        {
            base.RemoveEffect(effectToRemove);
            ResetMaxStaminaIfNowOver();
            ResetMaxMPIfNowOver();
        }

        public void AddSkill(ISkill skill)
        {
             m_skills.Add((Skill)skill);
        }

        private Weapon m_equipedWeapon;
        public override IWeapon CurrentWeapon
        {
            get
            {
                if (m_equipedWeapon == null)
                    return CoreGameEngine.Instance.ItemFactory.CreateMeleeWeapon(this);
                return m_equipedWeapon;
            }
        }

        private Weapon m_secondaryWeapon;
        public IWeapon SecondaryWeapon
        {
            get
            {
                if (m_secondaryWeapon == null)
                    return CoreGameEngine.Instance.ItemFactory.CreateMeleeWeapon(this);
                return m_secondaryWeapon;
            }
        }

        internal IItem Equip(IItem item)
        {
            if (item is IWeapon)
                return EquipWeapon((IWeapon)item);

            if (item is IArmor)
            {
                IArmor itemAsArmor = (IArmor)item;
                if (itemAsArmor.Type == "ChestArmor")
                {
                    IItem previousArmor = ChestArmor;
                    ChestArmor = (IArmor)item;
                    return previousArmor;
                }
                if (itemAsArmor.Type == "Helm")
                {
                    IItem previousArmor = Headpiece;
                    Headpiece = (IArmor)item;
                    return previousArmor;
                }
                if (itemAsArmor.Type == "Gloves")
                {
                    IItem previousArmor = Gloves;
                    Gloves = (IArmor)item;
                    return previousArmor;
                }
                if (itemAsArmor.Type == "Boots")
                {
                    IItem previousArmor = Boots;
                    Boots = (IArmor)item;
                    return previousArmor;
                }
            }

            throw new System.InvalidOperationException("Don't know how to equip - " + item.GetType());
        }

        internal IItem Unequip(IItem item)
        {
            if (item is IWeapon)
                return UnequipWeapon();

            if (item is IArmor)
            {
                IArmor itemAsArmor = (IArmor)item;
                if (itemAsArmor.Type == "ChestArmor")
                {
                    IItem previousArmor = ChestArmor;
                    ChestArmor = null;
                    ResetMaxStaminaIfNowOver();
                    return previousArmor;
                }
                if (itemAsArmor.Type == "Helm")
                {
                    IItem previousArmor = Headpiece;
                    Headpiece = null;
                    ResetMaxStaminaIfNowOver();
                    return previousArmor;
                }
                if (itemAsArmor.Type == "Gloves")
                {
                    IItem previousArmor = Gloves;
                    Gloves = null;
                    ResetMaxStaminaIfNowOver();
                    return previousArmor;
                }
                if (itemAsArmor.Type == "Boots")
                {
                    IItem previousArmor = Boots;
                    Boots = null;
                    ResetMaxStaminaIfNowOver();
                    return previousArmor;
                }
            }
            throw new System.InvalidOperationException("Don't know how to unequip - " + item.GetType());
        }

        private IWeapon EquipWeapon(IWeapon weapon)
        {
            if (weapon == null)
                throw new System.ArgumentException("EquipWeapon - Null weapon");

            Weapon oldWeapon = UnequipWeapon();

            m_equipedWeapon = (Weapon)weapon;
            if (m_equipedWeapon.IsRanged)
                m_equipedWeapon.LoadWeapon();

            return oldWeapon;
        }

        private Weapon UnequipWeapon()
        {
            if (m_equipedWeapon == null)
                return null;

            Weapon oldWeapon = (Weapon)m_equipedWeapon;
            m_equipedWeapon = null;
            return oldWeapon;
        }

        internal IWeapon EquipSecondaryWeapon(IWeapon weapon)
        {
            if (weapon == null)
                throw new System.ArgumentException("EquipSecondaryWeapon - Null weapon");

            IWeapon oldWeapon = UnequipSecondaryWeapon();

            m_secondaryWeapon = (Weapon)weapon;
            if (m_secondaryWeapon.IsRanged)
                m_secondaryWeapon.LoadWeapon();

            return oldWeapon;
        }

        public IWeapon UnequipSecondaryWeapon()
        {
            if (m_secondaryWeapon == null)
                return null;
            Weapon oldWeapon = (Weapon)m_secondaryWeapon;
            m_secondaryWeapon = null;
            return oldWeapon;
        }

        internal bool SwapPrimarySecondaryWeapons()
        {
            // If we're swapping to Melee and we're Melee
            if (CurrentWeapon.GetType() == typeof(MeleeWeapon) && SecondaryWeapon.GetType() == typeof(MeleeWeapon))
                return false;

            IWeapon mainWeapon = UnequipWeapon();
            IWeapon secondaryWeapon = UnequipSecondaryWeapon();
            
            if (secondaryWeapon != null)
                Equip(secondaryWeapon);
            
            if (mainWeapon != null)
                EquipSecondaryWeapon(mainWeapon);

            return true;
        }

        internal void TakeItem(Item i)
        {
            m_itemList.Add(i);
        }

        internal void RemoveItem(Item i)
        {
            m_itemList.Remove(i);
        }

        public override DiceRoll MeleeDamage
        {
            get
            {
                return new DiceRoll(7, 3);
            }
        }
        
        public override double MeleeCTCost
        {
            get
            {
                return 1.0;
            }
        }

        public override double Evade
        {
            get
            {
                return CombatDefenseCalculator.CalculateArmorEvade(this);
            }
        }

        public IList<EquipArmorReasons> CanNotEquipArmorReasons(IArmor armor)
        {
            List<EquipArmorReasons> reasonsList = new List<EquipArmorReasons>();
            if (armor.Type == "Boots" && ChestArmor != null)
            {
                // If I'm equiping boots and have chest armor, check to that the armor doesn't prevent that type
                if (((Item)ChestArmor).ContainsAttribute("RobesPreventBoots"))
                    reasonsList.Add(EquipArmorReasons.RobesPreventBoots);
            }
            if (armor.Type == "ChestArmor" && Boots != null)
            {
                // If I"m equipping chest armor that prevents wearing boots, make sure that I'm not already wearing some.
                if (((Item)armor).ContainsAttribute("RobesPreventBoots"))
                    reasonsList.Add(EquipArmorReasons.BootsPreventRobes);
            }

            switch (armor.Weight)
            {
                case ArmorWeight.Light:
                    break;
                case ArmorWeight.Standard:
                    if (!HasAttribute("StandardArmorProficiency"))
                        reasonsList.Add(EquipArmorReasons.Weight);
                    break;
                case ArmorWeight.Heavy:
                    if (!HasAttribute("HeavyArmorProficiency"))
                        reasonsList.Add(EquipArmorReasons.Weight);
                    break;
                default:
                    throw new System.InvalidOperationException("CouldEquip doesn't know how to handle type - " + armor.Weight.ToString());
            }
            return reasonsList;
        }

        public bool CanEquipArmor(IArmor armor)
        {
            return CanNotEquipArmorReasons(armor).Count == 0;
        }

        #region SaveLoad

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            base.ReadXml(reader);

            m_equipedWeapon = ReadWeaponFromSave(reader);
            m_secondaryWeapon = ReadWeaponFromSave(reader);

            m_currentHealth = reader.ReadElementContentAsInt();
            m_baseMaxHealth = reader.ReadElementContentAsInt();

            m_currentStamina = reader.ReadElementContentAsInt();
            m_baseMaxStamina = reader.ReadElementContentAsInt();

            m_baseCurrentMP = reader.ReadElementContentAsInt();
            m_baseMaxMP = reader.ReadElementContentAsInt();

            SkillPoints = reader.ReadElementContentAsInt();

            LastTurnSeenAMonster = reader.ReadElementContentAsInt();

            ChestArmor = (IArmor)Item.ReadXmlEntireNode(reader);
            Headpiece = (IArmor)Item.ReadXmlEntireNode(reader);
            Gloves = (IArmor)Item.ReadXmlEntireNode(reader);
            Boots = (IArmor)Item.ReadXmlEntireNode(reader);

            m_itemList = new List<Item>();
            ReadListFromXMLCore readItemDelegate = new ReadListFromXMLCore(delegate
            {
                string typeString = reader.ReadElementContentAsString();
                Item newItem = CoreGameEngine.Instance.ItemFactory.CreateBaseItem(typeString); 
                newItem.ReadXml(reader);
                m_itemList.Add(newItem);
            });
            ListSerialization.ReadListFromXML(reader, readItemDelegate);

            m_skills = new List<Skill>();
            ReadListFromXMLCore readSkillDelegate = new ReadListFromXMLCore(delegate
            {
                string skillName = reader.ReadElementContentAsString();
                m_skills.Add(SkillFactory.Instance.CreateSkill(skillName));
            });
            ListSerialization.ReadListFromXML(reader, readSkillDelegate);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Player");
            base.WriteXml(writer);

            WriteWeaponToSave(writer, m_equipedWeapon);
            WriteWeaponToSave(writer, m_secondaryWeapon);

            writer.WriteElementString("BaseCurrentHealth", m_currentHealth.ToString());
            writer.WriteElementString("BaseMaxHealth", m_baseMaxHealth.ToString());

            writer.WriteElementString("BaseCurrentStamina", m_currentStamina.ToString());
            writer.WriteElementString("BaseMaxStamina", m_baseMaxStamina.ToString());

            writer.WriteElementString("BaseCurrentMagic", m_baseCurrentMP.ToString());
            writer.WriteElementString("BaseMaxMagic", m_baseMaxMP.ToString());

            writer.WriteElementString("SkillPoints", SkillPoints.ToString());

            writer.WriteElementString("LastTurnSeenAMonster", LastTurnSeenAMonster.ToString());

            Item.WriteXmlEntireNode((Item)ChestArmor, "ChestArmor", writer);
            Item.WriteXmlEntireNode((Item)Headpiece, "Headpiece", writer);
            Item.WriteXmlEntireNode((Item)Gloves, "Gloves", writer);
            Item.WriteXmlEntireNode((Item)Boots, "Boots", writer);

            ListSerialization.WriteListToXML(writer, m_itemList, "Items");
            ListSerialization.WriteListToXML(writer, m_skills, "Skills");

            writer.WriteEndElement();
        }

        private Weapon ReadWeaponFromSave(XmlReader reader)
        {
            Weapon readWeapon = (Weapon)Item.ReadXmlEntireNode(reader);
            if (readWeapon is MeleeWeapon)
                ((MeleeWeapon)readWeapon).SetWielder(this);
            return readWeapon;
        }

        private void WriteWeaponToSave(XmlWriter writer, Weapon weapon)
        {
            Item.WriteXmlEntireNode(weapon, "EquipedWeapon", writer);
        }

        #endregion
    }
}
