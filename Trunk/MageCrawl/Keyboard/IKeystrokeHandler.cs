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
        public bool ShiftPressed;
        public bool AltPressed;

        public static NamedKey Invalid = new NamedKey();

        public NamedKey()
        {
            Code = (TCODKeyCode)(-1);
            Character = (char)0;
            ControlPressed = false;
            ShiftPressed = false;
            AltPressed = false;
        }

        public NamedKey(string name)
        {
            if (name.EndsWith("Control"))
            {
                ControlPressed = true;
                name = name.Remove(name.Length - 7);
            }
            if (name.EndsWith("Shift"))
            {
                ShiftPressed = true;
                name = name.Remove(name.Length - 5);
            }
            if (name.EndsWith("Alt"))
            {
                AltPressed = true;
                name = name.Remove(name.Length - 3);
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

            if (Character != (char)0)
            {
                return Character == other.Character && ControlPressed == other.ControlPressed  && AltPressed == other.AltPressed;
            }
            else
            {
                return Code == other.Code && ControlPressed == other.ControlPressed  && ShiftPressed == other.ShiftPressed && AltPressed == other.AltPressed;
            }
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
