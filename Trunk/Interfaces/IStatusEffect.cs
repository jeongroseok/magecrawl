using System;

namespace Magecrawl.Interfaces
{
    public interface IStatusEffect : INamedItem
    {
        string Name { get; }
        bool IsPositiveEffect { get; }
    }
}
