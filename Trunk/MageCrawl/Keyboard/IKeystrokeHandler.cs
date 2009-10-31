using System;
using libtcodWrapper;

namespace Magecrawl.Keyboard
{
    public interface IKeystrokeHandler
    {
        void HandleKeystroke(NamedKey keystroke);

        // Called when set as primary keyboard handler
        void NowPrimaried(object objOne, object objTwo, object objThree, object objFour);
    }

    public struct NamedKey
    {
        public KeyCode Code;
        public char Character;

        public static NamedKey Invalid = new NamedKey() 
        {
            Code = (KeyCode)(-1), Character = (char)0
        };

        public static NamedKey FromName(string name)
        {
            NamedKey key;
            try
            {
                key.Code = (KeyCode)Enum.Parse(typeof(KeyCode), name);
                key.Character = '\0';
            }
            catch (ArgumentException)
            {
                if (name.Length == 1)
                {
                    key.Code = KeyCode.TCODK_CHAR;
                    key.Character = name[0];
                }
                else
                {
                    throw new ArgumentException("Not a valid key name.", "name");
                }
            }
            return key;
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
            return base.GetHashCode();
        }

        public static bool operator ==(NamedKey lhs, NamedKey rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(NamedKey lhs, NamedKey rhs)
        {
            return !(lhs == rhs);
        }
    }
}
