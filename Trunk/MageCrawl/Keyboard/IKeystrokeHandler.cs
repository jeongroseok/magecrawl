using System;
using libtcod;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    public interface IKeystrokeHandler
    {
        void HandleKeystroke(NamedKey keystroke);

        // Called when set as primary keyboard handler
        void NowPrimaried(object request);
    }

    public class NamedKey
    {
        public TCODKeyCode Code;
        public char Character;
        public bool ControlPressed;

        public static NamedKey Invalid = new NamedKey();

        public NamedKey()
        {
            Code = (TCODKeyCode)(-1);
            Character = (char)0;
            ControlPressed = false;
        }

        public NamedKey(string name)
        {
            if (name.EndsWith("Control"))
            {
                ControlPressed = true;
                name = name.Remove(name.Length - 7);
            }

            if (name.Length > 1)
            {
                try
                {
                    Code = (TCODKeyCode)Enum.Parse(typeof(TCODKeyCode), name);
                    Character = '\0';
                }
                catch (ArgumentException)
                {
                    HandleCharacterElement(name);
                }
            }
            else
            {
                HandleCharacterElement(name);
            }
        }

        private void HandleCharacterElement(string name)
        {
            if (name.Length == 1)
            {
                Code = TCODKeyCode.Char;
                Character = name[0];
            }
            else
            {
                throw new ArgumentException("Not a valid key name.", "name");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            NamedKey other = (NamedKey)obj;

            return Code == other.Code && Character == other.Character && ControlPressed == other.ControlPressed;
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
            if (Code != (TCODKeyCode)0 && Code != TCODKeyCode.Char)
            {
                if (Code == TCODKeyCode.Up || Code == TCODKeyCode.Down || Code == TCODKeyCode.Left || Code == TCODKeyCode.Right)
                {
                    return "Arrow " + Code.ToString().Replace("TCODK_", "").ToLower().UpperCaseFirstLetter();
                }
                return Code.ToString().Replace("TCODK_", "").ToLower().UpperCaseFirstLetter();
            }
            return Character.ToString();
        }
    }
}
