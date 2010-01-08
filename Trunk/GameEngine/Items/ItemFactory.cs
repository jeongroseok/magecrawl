using System;
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

        internal ItemFactory()
        {
            LoadMappings();
        }

        public Item CreateItem(string name)
        {
            return m_itemMapping[name];
        }

        public Item CreateRandomItem()
        {
            using (TCODRandom random = new TCODRandom())
            {
                int targetLocation = random.GetRandomInt(0, m_itemMapping.Count - 1);
                string itemName = m_itemMapping.Keys.ToList()[targetLocation];
                return m_itemMapping[itemName];
            }
            throw new System.ArgumentOutOfRangeException("CreateRandomItem - Did not find random item?");
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
            {
                throw new System.InvalidOperationException("Bad weapons file");
            }
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
                    string damage = reader.GetAttribute("Damage");
                    string[] damageParts = damage.Split(',');
                    
                    short rolls = short.Parse(damageParts[0]);
                    short diceFaces = short.Parse(damageParts[1]);
                    short toAdd = short.Parse(damageParts[2]);
                    short multiplier = short.Parse(damageParts[3]);
                    DiceRoll damageRoll = new DiceRoll(rolls, diceFaces, toAdd, multiplier);

                    double ctCost = Double.Parse(reader.GetAttribute("CTCost"));
                    
                    string description = reader.GetAttribute("Description");
                    string flavorText = reader.GetAttribute("FlavorText");
                    m_itemMapping.Add(name, (Item)CreateWeaponCore(baseType, name, damageRoll, ctCost, description, flavorText));
                }
                if (reader.LocalName == "Potion")
                {
                    string name = reader.GetAttribute("Name");
                    string effectType = reader.GetAttribute("EffectType");

                    string strengthString = reader.GetAttribute("Strength");
                    int strength = int.Parse(strengthString);

                    string itemDescription = reader.GetAttribute("ItemDescription");
                    string flavorText = reader.GetAttribute("FlavorText");

                    m_itemMapping.Add(name, new Potion(name, effectType, strength, itemDescription, flavorText));
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
