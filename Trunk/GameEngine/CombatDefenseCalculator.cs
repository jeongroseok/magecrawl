using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine
{
    internal static class CombatDefenseCalculator
    {
        private const double BaseEvade = 5;

        public static double CalculateArmorEvade(IPlayer player)
        {
            return BaseEvade + GetArmorList(player).Sum(x => x.Evade);
        }

        public static int CalculateArmorStaminaBonus(IPlayer player)
        {
            return GetArmorList(player).Sum(x => x.StaminaBonus);
        }

        private static IEnumerable<IArmor> GetArmorList(IPlayer player)
        {
            List<IArmor> armorList = new List<IArmor>() { player.ChestArmor, player.Gloves, player.Boots, player.Headpiece};
            return armorList.Where(x => x != null);
        }

        private static ArmorWeight GetTotalArmorWeight(IPlayer player)
        {
            ArmorWeight largestWeight = ArmorWeight.None;
            foreach (IArmor a in GetArmorList(player))
            {
                if (a.Weight > largestWeight)
                    largestWeight = a.Weight;
            }
            return largestWeight;
        }
    }
}
