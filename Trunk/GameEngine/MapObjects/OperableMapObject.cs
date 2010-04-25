using System;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine.MapObjects
{
    internal abstract class OperableMapObject : MapObject
    {
        public abstract void Operate(ICharacter actor);
    }
}
