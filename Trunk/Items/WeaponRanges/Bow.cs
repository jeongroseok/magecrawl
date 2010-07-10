using System.Collections.Generic;
using System.Globalization;
using Magecrawl.Interfaces;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Items.WeaponRanges
{
    internal class Bow : RangedWeaponRangeBase, IWeaponVerb
    {
        public string AttackVerb
        {
            get
            {
                return "shoots an arrow at";
            }
        }

        public override string Name
        {
            get
            {
                return "Bow";
            }
        }
    }
}
