using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magecrawl.Interfaces
{
    public interface IWand
    {
        int Charges 
        {
            get;
        }
        
        int MaxCharges 
        {
            get;
        }
    }
}
