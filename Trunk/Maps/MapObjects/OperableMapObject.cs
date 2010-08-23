using System;
using Magecrawl.Interfaces;

namespace Magecrawl.Maps.MapObjects
{
    public abstract class OperableMapObject : MapObject
    {
        public abstract void Operate(ICharacter actor);
    }
}
