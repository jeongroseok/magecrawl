namespace Magecrawl.Interfaces
{
    public enum ArmorWeight
    {
        Light = 0,
        Standard = 1,
        Heavy = 2
    }
    public interface IArmor : IItem, INamedItem
    {
        ArmorWeight Weight
        {
            get;
        }

        int StaminaBonus
        {
            get;
        }

        double Evade
        {
            get;
        }
    }
}
