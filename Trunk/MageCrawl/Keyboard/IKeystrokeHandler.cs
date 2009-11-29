﻿using System;
using libtcodWrapper;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    public interface IKeystrokeHandler
    {
        void HandleKeystroke(NamedKey keystroke);

        // Called when set as primary keyboard handler
        void NowPrimaried(object objOne, object objTwo, object objThree, object objFour);
    }

    public class NamedKey
    {
        public KeyCode Code;
        public char Character;

        public static NamedKey Invalid = new NamedKey();

        public NamedKey()
        {
            Code = (KeyCode)(-1);
            Character = (char)0;
        }

        public NamedKey(string name)
        {
            try
            {
                Code = (KeyCode)Enum.Parse(typeof(KeyCode), name);
                Character = '\0';
            }
            catch (ArgumentException)
            {
                if (name.Length == 1)
                {
                    Code = KeyCode.TCODK_CHAR;
                    Character = name[0];
                }
                else
                {
                    throw new ArgumentException("Not a valid key name.", "name");
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            NamedKey other = (NamedKey)obj;

            return Code == other.Code && Character == other.Character;
        }

        public override int GetHashCode()
        {
            return (int)Code ^ (int)Character;
        }

        public static bool operator ==(NamedKey lhs, NamedKey rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(NamedKey lhs, NamedKey rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            if (Code != (KeyCode)0 && Code != KeyCode.TCODK_CHAR)
            {
                if (Code == KeyCode.TCODK_UP || Code == KeyCode.TCODK_DOWN || Code == KeyCode.TCODK_LEFT || Code == KeyCode.TCODK_RIGHT)
                {
                    return "Arrow " + Code.ToString().Replace("TCODK_", String.Empty).ToLower().UpperCaseFirstLetter();
                }
                return Code.ToString().Replace("TCODK_", String.Empty).ToLower().UpperCaseFirstLetter();
            }
            return Character.ToString();
        }
    }
}
