using System;
using Magecrawl.Interfaces;

namespace Magecrawl.GameEngine.MapObjects
{
    internal abstract class OperableMapObject : MapObject
    {
        public abstract void Operate(ICharacter actor);
    }
}
