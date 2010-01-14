namespace Magecrawl.GameEngine.Items
{
    interface IItemWithEffects
    {
        string Name
        {
            get;
        }

        string EffectType
        {
            get;
        }

        int Strength
        {
            get;
        }

        string OnUseString
        {
            get;
        }
    }
}
