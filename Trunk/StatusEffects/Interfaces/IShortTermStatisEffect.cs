using Magecrawl.EngineInterfaces;

namespace Magecrawl.StatusEffects.Interfaces
{
    public interface IShortTermStatusEffect : IStatusEffectCore
    {
        int CTLeft 
        {
            get;
        }

        void DecreaseCT(int previousCT, int currentCT);
    }
}
