using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IItem
    {
        string Name
        {
            get;
        }

        string ItemDescription
        {
            get;
        }

        string FlavorDescription
        {
            get;
        }

        List<ItemOptions> PlayerOptions
        {
            get;
        }
    }
}
