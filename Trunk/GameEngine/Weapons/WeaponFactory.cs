using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Weapons
{
    internal sealed class WeaponFactory
    {
        private Dictionary<string, IWeapon> m_weaponMapping;

        internal WeaponFactory()
        {
            LoadMappings();
        }

        public IWeapon CreateWeapon(string name)
        {
            return m_weaponMapping[name];
        }

        private void LoadMappings()
        {
            m_weaponMapping = new Dictionary<string, IWeapon>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(new StreamReader("Weapons.xml"), settings);
            reader.Read();  // XML declaration
            reader.Read();  // KeyMappings element
            if (reader.LocalName != "Weapons")
            {
                throw new System.InvalidOperationException("Bad weapons file");
            }
            while (true)
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Weapons")
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
                    
                    string description = reader.GetAttribute("Description");
                    string flavorText = reader.GetAttribute("FlavorText");
                    m_weaponMapping.Add(name, CreateWeapon(baseType, name, damageRoll, description, flavorText));
                }
            }
            reader.Close();
        }

        private IWeapon CreateWeapon(string typeName, string name, DiceRoll damage, string description, string flavorTest)
        {
            Assembly weaponsAssembly = this.GetType().Assembly;
            Type type = weaponsAssembly.GetType("Magecrawl.GameEngine.Weapons." + typeName);
            if (type != null)
            {
                return Activator.CreateInstance(type, name, damage, description, flavorTest) as IWeapon;
            }
            else
            {
                throw new ArgumentException("CreateWeapon - don't know how to make: " + typeName);
            }
        }
    }
}
