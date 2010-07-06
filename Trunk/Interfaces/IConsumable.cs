using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.Interfaces
{
    public interface IConsumable
    {
        int Charges { get; }
        int MaxCharges { get; }
    }
}
