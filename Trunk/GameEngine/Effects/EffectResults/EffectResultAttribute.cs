using System;

namespace Magecrawl.GameEngine.Effects.EffectResults
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class EffectResultAttribute : Attribute
    {
        public string Tag { get; private set; }

        public EffectResultAttribute(string tag)
        {
            Tag = tag;
        }
    }
}
