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
        private const double BaseEvade = 10;

        public static double CalculateArmorEvade(IPlayer player)
        {
            return BaseEvade + GetArmorList(player).Sum(x => x.Evade);
        }

        public static int CalculateArmorStaminaBonus(IPlayer player)
        {
            return GetArmorList(player).Sum(x => x.StaminaBonus);
        }

        public static ArmorWeight GetTotalArmorWeight(IPlayer player)
        {
            ArmorWeight largestWeight = ArmorWeight.Light;
            foreach (IArmor a in GetArmorList(player))
            {
                if (a.Weight > largestWeight)
                    largestWeight = a.Weight;
            }
            return largestWeight;
        }
       
        public static int GetMonsterVisionBonus(IPlayer player)
        {
            switch(GetTotalArmorWeight(player))
            {
                case ArmorWeight.Heavy:
                    return 1;
                case ArmorWeight.Standard:
                case ArmorWeight.Light:
                default:
                    return 0;
            }
        }

        private static double GetArmorCostPenality(IArmor armor)
        {
            const double StandardWeightPenality = .2;
            const double HeavyWeightPenality = .4;

            switch(armor.Weight)
            {
                case ArmorWeight.Standard:
                    return StandardWeightPenality;
                case ArmorWeight.Heavy:
                    return HeavyWeightPenality;
                case ArmorWeight.Light:
                default:
                    return 0;
            }
        }

        public static double CalculateSpellPenalityForArmor(IPlayer player)
        {
            return 1.0 + GetArmorList(player).Sum(x => GetArmorCostPenality(x));
        }

        private static IEnumerable<IArmor> GetArmorList(IPlayer player)
        {
            List<IArmor> armorList = new List<IArmor>() { player.ChestArmor, player.Gloves, player.Boots, player.Headpiece};
            return armorList.Where(x => x != null);
        }
    }
}
