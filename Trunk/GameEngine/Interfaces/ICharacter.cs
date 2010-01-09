using System;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface ICharacter
    {
        string Name
        {
            get;
        }

        Point Position
        {
            get;
        }

        int CurrentHP
        {
            get;
        }

        int MaxHP
        {
            get;
        }

        int Vision
        {
            get;
        }

        int UniqueID
        {
            get;
        }        

        IWeapon CurrentWeapon
        {
            get;
        }

        DiceRoll MeleeDamage
        {
            get;
        }

        double MeleeSpeed
        {
            get;
        }
    }
}
