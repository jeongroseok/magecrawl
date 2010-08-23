using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.EngineInterfaces
{
    public interface ITreasureGenerator
    {
        void GenerateTreasureChestTreasure(ICharacter actor, Point position);
    }
}
