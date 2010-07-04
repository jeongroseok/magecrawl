using System;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public interface ICharacter : INamedItem
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

        IWeapon SecondaryWeapon
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

        double Evade
        {
            get;
        }

        bool IsAlive
        {
            get;
        }
    }
}
