using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MageCrawl.Silverlight.KeyboardHandlers
{
    public static class KeyboardConverter
    {
        // Taken from http://virtu.codeplex.com/SourceControl/changeset/view/78ebeb6c57d2#Virtu%2fSilverlight%2fServices%2fSilverlightKeyboardService.cs
        // with permission from fool with changes
        public static MagecrawlKey GetConvertedKey(Key key, int platformKeyCode)
        {
            var modifiers = System.Windows.Input.Keyboard.Modifiers;
            bool shift = ((modifiers & ModifierKeys.Shift) != 0);

            switch (key)
            {
                default:
                    return (MagecrawlKey)key;
                case Key.D1:                                    
                    return shift ? MagecrawlKey.Exclamation : MagecrawlKey.D1;
                case Key.D2:
                    return shift ? MagecrawlKey.At : MagecrawlKey.D2;
                case Key.D3:
                    return shift ? MagecrawlKey.Pound : MagecrawlKey.D3;
                case Key.D4:
                    return shift ? MagecrawlKey.Dollar : MagecrawlKey.D4;
                case Key.D5:
                    return shift ? MagecrawlKey.Percent : MagecrawlKey.D5;
                case Key.D6:
                    return shift ? MagecrawlKey.Carrot : MagecrawlKey.D6;
                case Key.D7:
                    return shift ? MagecrawlKey.Ampersand : MagecrawlKey.D7;
                case Key.D8:
                    return shift ? MagecrawlKey.Asterisk : MagecrawlKey.D8;
                case Key.D9:
                    return shift ? MagecrawlKey.LeftParen : MagecrawlKey.D9;
                case Key.D0:
                    return shift ? MagecrawlKey.RightParen : MagecrawlKey.D0;
                case Key.A:
                    return shift ? MagecrawlKey.A : MagecrawlKey.a;
                case Key.B:
                    return shift ? MagecrawlKey.B : MagecrawlKey.b;
                case Key.C:
                    return shift ? MagecrawlKey.C : MagecrawlKey.c;
                case Key.D:
                    return shift ? MagecrawlKey.D : MagecrawlKey.d;
                case Key.E:
                    return shift ? MagecrawlKey.E : MagecrawlKey.e;
                case Key.F:
                    return shift ? MagecrawlKey.F : MagecrawlKey.f;
                case Key.G:
                    return shift ? MagecrawlKey.G : MagecrawlKey.g;
                case Key.H:
                    return shift ? MagecrawlKey.H : MagecrawlKey.h;
                case Key.I:
                    return shift ? MagecrawlKey.I : MagecrawlKey.i;
                case Key.J:
                    return shift ? MagecrawlKey.J : MagecrawlKey.j;
                case Key.K:
                    return shift ? MagecrawlKey.K : MagecrawlKey.k;
                case Key.L:
                    return shift ? MagecrawlKey.L : MagecrawlKey.l;
                case Key.M:
                    return shift ? MagecrawlKey.M : MagecrawlKey.m;
                case Key.N:
                    return shift ? MagecrawlKey.N : MagecrawlKey.n;
                case Key.O:
                    return shift ? MagecrawlKey.O : MagecrawlKey.o;
                case Key.P:
                    return shift ? MagecrawlKey.P : MagecrawlKey.p;
                case Key.Q:
                    return shift ? MagecrawlKey.Q : MagecrawlKey.q;
                case Key.R:
                    return shift ? MagecrawlKey.R : MagecrawlKey.r;
                case Key.S:
                    return shift ? MagecrawlKey.S : MagecrawlKey.s;
                case Key.T:
                    return shift ? MagecrawlKey.T : MagecrawlKey.t;
                case Key.U:
                    return shift ? MagecrawlKey.U : MagecrawlKey.u;
                case Key.V:
                    return shift ? MagecrawlKey.V : MagecrawlKey.v;
                case Key.W:
                    return shift ? MagecrawlKey.W : MagecrawlKey.w;
                case Key.X:
                    return shift ? MagecrawlKey.X : MagecrawlKey.x;
                case Key.Y:
                    return shift ? MagecrawlKey.Y : MagecrawlKey.y;
                case Key.Z:
                    return shift ? MagecrawlKey.Z : MagecrawlKey.z;
                case Key.Unknown:
                    {
                        switch (Environment.OSVersion.Platform)
                        {
                            case PlatformID.Win32NT:
                            {
                                switch (platformKeyCode)
                                {
                                    case 0xBA: // WinForms Keys.Oem1
                                        return shift ? MagecrawlKey.Colon : MagecrawlKey.Semicolon;
                                    case 0xBF: // WinForms Keys.Oem2
                                        return shift ? MagecrawlKey.Question : MagecrawlKey.Slash;
                                    case 0xC0: // WinForms Keys.Oem3
                                        return shift ? MagecrawlKey.Tilde  : MagecrawlKey.Backquote;
                                    case 0xDB: // WinForms Keys.Oem4
                                        return shift ? MagecrawlKey.LeftCurlyBrace : MagecrawlKey.LeftSquareBrace;
                                    case 0xDC: // WinForms Keys.Oem5
                                        return shift ? MagecrawlKey.Pipe : MagecrawlKey.Backslash;
                                    case 0xDD: // WinForms Keys.Oem6
                                        return shift ? MagecrawlKey.RightCurlyBrace : MagecrawlKey.RightSquareBrace;
                                    case 0xDE: // WinForms Keys.Oem7
                                        return shift ? MagecrawlKey.Quote : MagecrawlKey.Apostrophe;
                                    case 0xBD: // WinForms Keys.OemMinus
                                        return shift ? MagecrawlKey.Underscore : MagecrawlKey.Subtract;
                                    case 0xBB: // WinForms Keys.OemPlus
                                        return shift ? MagecrawlKey.Add : MagecrawlKey.Equals;
                                    case 0xBC: // WinForms Keys.OemComma
                                        return shift ? MagecrawlKey.LessThan : MagecrawlKey.Comma;
                                    case 0xBE: // WinForms Keys.OemPeriod
                                        return shift ? MagecrawlKey.GreaterThan : MagecrawlKey.Period;
                                }
                                break;
                            }
                            case PlatformID.MacOSX:
                            {
                                switch (platformKeyCode)
                                {
                                    case 0x29:
                                        return shift ? MagecrawlKey.Colon : MagecrawlKey.Semicolon;
                                    case 0x2C:
                                        return shift ? MagecrawlKey.Question : MagecrawlKey.Slash;
                                    case 0x32:
                                        return shift ? MagecrawlKey.Tilde  : MagecrawlKey.Backquote;
                                    case 0x21:
                                        return shift ? MagecrawlKey.LeftCurlyBrace : MagecrawlKey.LeftSquareBrace;
                                    case 0x2A:
                                        return shift ? MagecrawlKey.Pipe : MagecrawlKey.Backslash;
                                    case 0x1E:
                                        return shift ? MagecrawlKey.RightCurlyBrace : MagecrawlKey.RightSquareBrace;
                                    case 0x27:
                                        return shift ? MagecrawlKey.Quote : MagecrawlKey.Apostrophe;
                                    case 0x1B:
                                        return shift ? MagecrawlKey.Underscore : MagecrawlKey.Subtract;
                                    case 0x18:
                                        return shift ? MagecrawlKey.Add : MagecrawlKey.Equals;
                                    case 0x2B:
                                        return shift ? MagecrawlKey.LessThan : MagecrawlKey.Comma;
                                    case 0x2F:
                                        return shift ? MagecrawlKey.GreaterThan : MagecrawlKey.Period;
                                }
                                break;
                            }
                            case PlatformID.Unix:
                            {
                                switch (platformKeyCode)
                                {
                                    case 0x2F:
                                        return shift ? MagecrawlKey.Colon : MagecrawlKey.Semicolon;
                                    case 0x3D:
                                        return shift ? MagecrawlKey.Question : MagecrawlKey.Slash;
                                    case 0x31:
                                        return shift ? MagecrawlKey.Tilde  : MagecrawlKey.Backquote;
                                    case 0x22:
                                        return shift ? MagecrawlKey.LeftCurlyBrace : MagecrawlKey.LeftSquareBrace;
                                    case 0x33:
                                        return shift ? MagecrawlKey.Pipe : MagecrawlKey.Backslash;
                                    case 0x23:
                                        return shift ? MagecrawlKey.RightCurlyBrace : MagecrawlKey.RightSquareBrace;
                                    case 0x30:
                                        return shift ? MagecrawlKey.Quote : MagecrawlKey.Apostrophe;
                                    case 0x14:
                                        return shift ? MagecrawlKey.Underscore : MagecrawlKey.Subtract;
                                    case 0x15:
                                        return shift ? MagecrawlKey.Add : MagecrawlKey.Equals;
                                    case 0x3B:
                                        return shift ? MagecrawlKey.LessThan : MagecrawlKey.Comma;
                                    case 0x3C:
                                        return shift ? MagecrawlKey.GreaterThan : MagecrawlKey.Period;
                                }
                                break;
                            }
                        }
                        break;
                    }
            }
            return MagecrawlKey.None;
        }
    }
}
