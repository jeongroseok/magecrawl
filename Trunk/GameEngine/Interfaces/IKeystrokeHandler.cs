using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcodWrapper;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IKeystrokeHandler
    {
        KeystrokeResult HandleKeystroke(NamedKey keystroke);
    }

    // Let's the rest of the GUI know what if anything to expect
    // BCL: this enum is now in the core. It's bad that all code everywhere knows about the different states that can be
    // returned, and I don't think this enum should have to exist anyway.
    public enum KeystrokeResult
    {
        None,        // We didn't handle anything, no keypresses or those that shouldn't matter
        Action,      // Soemthing happened, need to update map with new data
        InOperate,   // Hit operate key, next arrow will try to operate that way
        InAttack,    // Hit attack key, next arrow will try to attack that way
        InRangedAttack, // Started ranged attack; subsequent arrow keys will move the target selection
        DebuggingMoveableOnOff,  // Turned 'pathable' debugging view on/off
        TextBoxClear,   // Text Box Cleared of history
        TextBoxUp,      // Scroll Text Box Up
        TextBoxDown,    // Scroll Text Box Down
        Quit            // Asked game to quit.
    }

    public struct NamedKey
    {
        public KeyCode Code;
        public char Character;

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
    }
}
