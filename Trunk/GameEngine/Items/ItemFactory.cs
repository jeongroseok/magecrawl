﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Items
{
    internal sealed class ItemFactory
    {
        private Dictionary<string, Item> m_itemMapping;
        private static TCODRandom m_random;

        static ItemFactory()
        {
            m_random = new TCODRandom();
        }

        internal ItemFactory()
        {
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
            int targetLocation = m_random.GetRandomInt(0, m_itemMapping.Count - 1);
            string itemName = m_itemMapping.Keys.ToList()[targetLocation];
            return CreateItem(itemName);
        }

        private void LoadMappings()
        {
            m_itemMapping = new Dictionary<string, Item>();

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
                }
                if (reader.LocalName == "Potion" || reader.LocalName == "Scroll")
                {
                    bool potion = reader.LocalName == "Potion";
                    string name = reader.GetAttribute("Name");
                    string effectType = reader.GetAttribute("EffectType");

                    int strength = int.Parse(reader.GetAttribute("Strength"));

                    string itemDescription = reader.GetAttribute("ItemDescription");
                    string flavorText = reader.GetAttribute("FlavorText");

                    if (potion)
                        m_itemMapping.Add(name, new Potion(name, effectType, strength, itemDescription, flavorText));
                    else
                        m_itemMapping.Add(name, new Scroll(name, effectType, strength, itemDescription, flavorText));
                }
                if (reader.LocalName == "Wand")
                {
                    string name = reader.GetAttribute("Name");
                    string effectType = reader.GetAttribute("EffectType");

                    int strength = int.Parse(reader.GetAttribute("Strength"));

                    string itemDescription = reader.GetAttribute("ItemDescription");
                    string flavorText = reader.GetAttribute("FlavorText");

                    DiceRoll startingCharges = new DiceRoll(reader.GetAttribute("StartCharges"));
                    int maxNumberCharges = int.Parse(reader.GetAttribute("MaxCharges"));

                    m_itemMapping.Add(name, new Wand(name, effectType, strength, itemDescription, flavorText, maxNumberCharges, startingCharges));
                }
            }
            reader.Close();
        }

        private IWeapon CreateWeaponCore(string typeName, string name, DiceRoll damage, double ctCost, string description, string flavorTest)
        {
            Assembly weaponsAssembly = this.GetType().Assembly;
            Type type = weaponsAssembly.GetType("Magecrawl.GameEngine.Weapons." + typeName);
            if (type != null)
            {
                return Activator.CreateInstance(type, name, damage, ctCost, description, flavorTest) as IWeapon;
            }
            else
            {
                throw new ArgumentException("CreateWeapon - don't know how to make: " + typeName);
            }
        }
    }
}
