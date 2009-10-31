using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.GameEngine.Interfaces
{
    // A 'named' item, one that can be stuck in item lists
    public interface INamedItem
    {
        string DisplayName
        {
            get;
        }
    }
}
