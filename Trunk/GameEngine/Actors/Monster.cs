using System;
using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.SaveLoad;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors
{
    internal enum MonsterAction
    {
        DidAction,
        DidMove
    }

    internal sealed class Monster : Character
    {
        // Share one RNG between monsters
        private static TCODRandom m_random;
        static Monster()
        {
            m_random = new TCODRandom();
        }

        public Monster()
        {
            m_position = new Point(-1, -1);
            m_CT = 0;
        }

        public Monster(int x, int y)
        {
            m_position = new Point(x, y);
            m_CT = 0;
        }

        internal MonsterAction Action(CoreGameEngine engine)
        {
            for (int i = 0; i < 10; ++i)
            {
                Direction directionToTry = (Direction)m_random.GetRandomInt(1, 8);
                if (engine.Move(this, directionToTry))
                {
                    return MonsterAction.DidMove;
                }
            }
            
            // If nothing else, 'wait'
            return MonsterAction.DidAction;
        }

        #region SaveLoad
        public override System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            m_position = m_position.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", "Monster");
            Position.WriteToXml(writer, "Position");
        }

        internal static Monster CreateMonsterObjectFromTypeString(string s)
        {
            switch (s)
            {
                case "Monster":
                    return new Monster();
                default:
                    throw new System.ArgumentException("Invalid type in CreateMonsterObjectFromTypeString");
            }
        }

        #endregion
    }
}
