using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class ItemFactory
    {
        private Dictionary<string, Item> m_itemMapping;
        private HashSet<Item> m_itemsNotToDrop;
        private TCODRandom m_random;

        internal ItemFactory()
        {
            m_random = new TCODRandom();
            LoadMappings();
        }

        public Item CreateItem(string name)
        {
            Item newItem = (Item)m_itemMapping[name].Clone();

            Wand newItemAsWand = newItem as Wand;
            if (newItemAsWand != null)
                newItemAsWand.Charges = newItemAsWand.NewWandPossibleCharges.Roll();
            return newItem;
        }

        public Item CreateRandomItem()
        {
            while (true)
            {
                int targetLocation = m_random.GetRandomInt(0, m_itemMapping.Count - 1);
                string itemName = m_itemMapping.Keys.ToList()[targetLocation];
                if (!m_itemsNotToDrop.Contains(m_itemMapping[itemName]))
                    return CreateItem(itemName);
            }
        }

        private void LoadMappings()
        {
            // Save off previous culture and switch to invariant for serialization.
            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_itemMapping = new Dictionary<string, Item>();
            m_itemsNotToDrop = new HashSet<Item>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader(Path.Combine("Resources", "Items.xml")), settings);
            reader.Read();  // XML declaration
            reader.Read();  // Items element
            if (reader.LocalName != "Items")
                throw new System.InvalidOperationException("Bad weapons file");
            
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Items")
                {
                    break;
                }
                if (reader.LocalName == "Weapon")
                {
                    string name = reader.GetAttribute("Name");
                    string baseType = reader.GetAttribute("BaseType");

                    DiceRoll damage = new DiceRoll(reader.GetAttribute("Damage"));

                    double ctCost = Double.Parse(reader.GetAttribute("CTCost"));
                    
                    string description = reader.GetAttribute("Description");
                    string flavorText = reader.GetAttribute("FlavorText");
                    m_itemMapping.Add(name, (Item)CreateWeaponCore(baseType, name, damage, ctCost, description, flavorText));

                    CheckForNotDropList(reader, name);
                }
                if (reader.LocalName == "Armor")
                {
                    string name = reader.GetAttribute("Name");
                    string baseType = reader.GetAttribute("BaseType");

                    string description = reader.GetAttribute("ItemDescription");
                    string flavorText = reader.GetAttribute("FlavorText");
                    ArmorWeight weight = (ArmorWeight)Enum.Parse(typeof(ArmorWeight), reader.GetAttribute("ArmorWeight"));

                    double defense = double.Parse(reader.GetAttribute("Defense"));
                    double evade = double.Parse(reader.GetAttribute("Evade"));

                    m_itemMapping.Add(name, (Item)CreateArmorCore(baseType, name, weight, defense, evade, description, flavorText));

                    CheckForNotDropList(reader, name);
                }
                if (reader.LocalName == "Potion" || reader.LocalName == "Scroll" || reader.LocalName == "Supplies")
                {
                    bool potion = reader.LocalName == "Potion";
                    bool supplies = reader.LocalName == "Supplies";
                    bool scroll = reader.LocalName == "Scroll";
                    
                    string name = reader.GetAttribute("Name");
                    string effectType = reader.GetAttribute("EffectType");

                    string targettingType = reader.GetAttribute("TargettingType");
                    string range = reader.GetAttribute("Range");
                    if (targettingType == null)
                        targettingType = "Self";
                    else
                        targettingType += ":" + range;

                    int strength = int.Parse(reader.GetAttribute("Strength"));

                    string itemDescription = reader.GetAttribute("ItemDescription");
                    string flavorText = reader.GetAttribute("FlavorText");

                    if (potion)
                        m_itemMapping.Add(name, new Potion(name, effectType, targettingType, strength, itemDescription, flavorText));
                    else if (scroll)
                        m_itemMapping.Add(name, new Scroll(name, effectType, targettingType, strength, itemDescription, flavorText));
                    else if (supplies)
                        m_itemMapping.Add(name, new Supplies(name, effectType, targettingType, strength, itemDescription, flavorText));
                    else
                        throw new Exception("Unknown type in ItemFaction");

                    CheckForNotDropList(reader, name);
                }
                if (reader.LocalName == "Wand")
                {
                    string name = reader.GetAttribute("Name");
                    string effectType = reader.GetAttribute("EffectType");

                    string targettingType = reader.GetAttribute("TargettingType");
                    string range = reader.GetAttribute("Range");
                    if (targettingType == null)
                        targettingType = "Self";
                    else
                        targettingType += ":" + range;

                    int strength = int.Parse(reader.GetAttribute("Strength"));

                    string itemDescription = reader.GetAttribute("ItemDescription");
                    string flavorText = reader.GetAttribute("FlavorText");

                    DiceRoll startingCharges = new DiceRoll(reader.GetAttribute("StartCharges"));
                    int maxNumberCharges = int.Parse(reader.GetAttribute("MaxCharges"));

                    m_itemMapping.Add(name, new Wand(name, effectType, targettingType, strength, itemDescription, flavorText, maxNumberCharges, startingCharges));

                    CheckForNotDropList(reader, name);
                }
            }
            reader.Close();

            Thread.CurrentThread.CurrentCulture = previousCulture; 
        }

        private void CheckForNotDropList(XmlReader reader, string name)
        {
            string doNotDropString = reader.GetAttribute("DoNotDrop");
            if (doNotDropString != null && doNotDropString == "True")
                m_itemsNotToDrop.Add(m_itemMapping[name]);
        }

        private IWeapon CreateWeaponCore(string typeName, string name, DiceRoll damage, double ctCost, string description, string flavorText)
        {
            return (IWeapon)Activator.CreateInstance(GetTypeToMake("Magecrawl.GameEngine.Weapons", typeName), name, damage, ctCost, description, flavorText);
        }

        private IArmor CreateArmorCore(string typeName, string name, ArmorWeight weight, double defense, double evade, string description, string flavorText)
        {
            return (IArmor)Activator.CreateInstance(GetTypeToMake("Magecrawl.GameEngine.Armor", typeName), name, weight, defense, evade, description, flavorText);
        }

        private Type GetTypeToMake(string space, string typeName)
        {
            Assembly thisAssembly = this.GetType().Assembly;
            Type type = thisAssembly.GetType(space + "." + typeName);
            if (type != null)
                return type;
            else
                throw new ArgumentException("CreateItemFromNamespace - don't know how to make: " + typeName);
        }
    }
}
