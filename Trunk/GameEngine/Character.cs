﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine
{
    public abstract class Character : Interfaces.ICharacter, IXmlSerializable
    {
        protected Point m_position;
        protected int m_CT;

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


        public Point Position
        {
            get
            {
                return m_position;
            }
            internal set
            {
                m_position = value;
            }
        }

        public int CT
        {
            get
            {
                return m_CT;
            }
            internal set
            {
                m_CT = value;
            }
        }

        public abstract System.Xml.Schema.XmlSchema GetSchema();
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
    }
}
