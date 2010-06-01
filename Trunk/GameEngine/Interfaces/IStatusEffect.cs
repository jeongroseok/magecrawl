using System;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IStatusEffect
    {
        string Name { get; }
        bool IsPositiveEffect { get; }
    }
}
