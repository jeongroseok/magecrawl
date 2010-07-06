using System;
using Magecrawl.Items.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.Items.WeaponRanges
{
    internal static class WeaponRangeFactory
    {
        internal static IWeaponRange Create(string name)
        {
            Type typeToCreate = TypeLocator.GetTypeToMake(typeof(WeaponRangeFactory), "Magecrawl.Items.WeaponRanges", name);
            return (IWeaponRange)Activator.CreateInstance(typeToCreate);
        }
    }
}
