using System;
using libtcodWrapper;

namespace Magecrawl.Keyboard
{
    public interface IKeystrokeHandler
    {
        void HandleKeystroke(NamedKey keystroke);

        // Called when set as primary keyboard handler
        void NowPrimaried(object objOne, object objTwo, object objThree);
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
