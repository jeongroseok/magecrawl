using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Interfaces
{
    public interface IItem : INamedItem
    {
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

        string ItemEffectSchool
        {
            get;
        }
    }
}
