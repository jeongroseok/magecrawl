using System;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IStatusEffect : INamedItem
    {
        string Name { get; }
        bool IsPositiveEffect { get; }
    }
}
