using Magecrawl.Interfaces;
using System.Collections.Generic;

namespace Magecrawl.EngineInterfaces
{
    public interface ICharacterCore : ICharacter
    {
        void AddEffect(IStatusEffectCore effectToAdd);
        void DamageJustStamina(int dmg);
        int Heal(int toHeal, bool magical);

        IEnumerable<IStatusEffectCore> Effects
        {
            get;
        }
    }
}
