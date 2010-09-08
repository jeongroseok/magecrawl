using System;
using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
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

        string Type
        {
            get;
        }
    }
}
